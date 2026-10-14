using UnityEngine;
using StickmanParty.Player;

namespace StickmanParty.Minigames.Knockout
{
    public class ShrinkingPlatform : MonoBehaviour
    {
        [SerializeField] private float initialRadius = 5.5f;
        [SerializeField] private float minRadius = 2.0f;
        [SerializeField] private float shrinkSpeed = 0.08f;

        public float CurrentRadius { get; private set; }
        private LineRenderer ringRenderer;
        private SpriteRenderer fillRenderer;

        private void Awake()
        {
            CurrentRadius = initialRadius;
            SetupVisualRing();
        }

        private void SetupVisualRing()
        {
            ringRenderer = gameObject.AddComponent<LineRenderer>();
            ringRenderer.useWorldSpace = false;
            ringRenderer.loop = true;
            ringRenderer.startWidth = 0.15f;
            ringRenderer.endWidth = 0.15f;
            ringRenderer.material = new Material(Shader.Find("Sprites/Default"));
            ringRenderer.startColor = new Color(1f, 0.4f, 0.1f, 0.9f);
            ringRenderer.endColor = new Color(1f, 0.4f, 0.1f, 0.9f);

            int segments = 48;
            ringRenderer.positionCount = segments;
            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                ringRenderer.SetPosition(i, new Vector3(Mathf.Cos(angle) * CurrentRadius, Mathf.Sin(angle) * CurrentRadius, 0f));
            }
        }

        public void Shrink(float delta)
        {
            if (CurrentRadius > minRadius)
            {
                CurrentRadius = Mathf.Max(minRadius, CurrentRadius - (shrinkSpeed * delta));
                UpdateVisualRing();
            }
        }

        private void UpdateVisualRing()
        {
            if (ringRenderer == null) return;
            int segments = ringRenderer.positionCount;
            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                ringRenderer.SetPosition(i, new Vector3(Mathf.Cos(angle) * CurrentRadius, Mathf.Sin(angle) * CurrentRadius, 0f));
            }
        }

        public bool IsInsidePlatform(Vector2 position)
        {
            return position.magnitude <= CurrentRadius;
        }
    }
}
