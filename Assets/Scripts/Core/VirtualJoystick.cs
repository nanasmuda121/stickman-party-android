using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StickmanParty.Core
{
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private float handleRange = 50f;
        public Vector2 InputVector { get; private set; } = Vector2.zero;

        private RectTransform bgRect;
        private RectTransform handleRect;

        private void Awake()
        {
            bgRect = GetComponent<RectTransform>();
            Transform handle = transform.Find("Handle");
            if (handle != null)
            {
                handleRect = handle.GetComponent<RectTransform>();
            }
        }

        public void Setup(Color playerColor)
        {
            if (bgRect == null) bgRect = GetComponent<RectTransform>();

            // Background circle
            var bgImg = GetComponent<Image>();
            if (bgImg == null) bgImg = gameObject.AddComponent<Image>();
            bgImg.color = new Color(playerColor.r, playerColor.g, playerColor.b, 0.35f);

            // Handle circle
            if (handleRect == null)
            {
                GameObject handleObj = new GameObject("Handle");
                handleObj.transform.SetParent(transform, false);
                handleRect = handleObj.AddComponent<RectTransform>();
                handleRect.sizeDelta = new Vector2(50, 50);

                var handleImg = handleObj.AddComponent<Image>();
                handleImg.color = new Color(playerColor.r, playerColor.g, playerColor.b, 0.85f);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgRect, eventData.position, eventData.pressEventCamera, out pos))
            {
                pos.x = (pos.x / bgRect.sizeDelta.x);
                pos.y = (pos.y / bgRect.sizeDelta.y);

                InputVector = new Vector2(pos.x * 2f, pos.y * 2f);
                InputVector = (InputVector.magnitude > 1.0f) ? InputVector.normalized : InputVector;

                if (handleRect != null)
                {
                    handleRect.anchoredPosition = new Vector2(InputVector.x * (bgRect.sizeDelta.x / 2.5f), InputVector.y * (bgRect.sizeDelta.y / 2.5f));
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            InputVector = Vector2.zero;
            if (handleRect != null)
            {
                handleRect.anchoredPosition = Vector2.zero;
            }
        }
    }
}
