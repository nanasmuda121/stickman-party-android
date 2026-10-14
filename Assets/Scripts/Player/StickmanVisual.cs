using UnityEngine;

namespace StickmanParty.Player
{
    public class StickmanVisual : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer headRenderer;
        [SerializeField] private SpriteRenderer bodyRenderer;
        [SerializeField] private SpriteRenderer leftEyeRenderer;
        [SerializeField] private SpriteRenderer rightEyeRenderer;
        [SerializeField] private SpriteRenderer leftHandRenderer;
        [SerializeField] private SpriteRenderer rightHandRenderer;

        private Color playerColor = Color.red;
        private Transform visualRoot;
        private float bobTimer = 0f;
        private bool isPunching = false;
        private float punchProgress = 0f;

        private void Awake()
        {
            visualRoot = transform.Find("VisualRoot");
            if (visualRoot == null)
            {
                visualRoot = new GameObject("VisualRoot").transform;
                visualRoot.SetParent(transform, false);
            }

            CreateProceduralSpritesIfMissing();
        }

        public void SetColor(Color color)
        {
            playerColor = color;
            if (headRenderer != null) headRenderer.color = color;
            if (bodyRenderer != null) bodyRenderer.color = color;
            if (leftHandRenderer != null) leftHandRenderer.color = color;
            if (rightHandRenderer != null) rightHandRenderer.color = color;
        }

        public void UpdateAnimation(Vector2 moveInput, float currentSpeed, bool isMoving)
        {
            if (visualRoot == null) return;

            // Bouncy squash & stretch run cycle
            if (isMoving && currentSpeed > 0.1f)
            {
                bobTimer += Time.deltaTime * (currentSpeed * 4f);
                float bounceY = Mathf.Abs(Mathf.Sin(bobTimer)) * 0.15f;
                float squashX = 1f + (Mathf.Sin(bobTimer * 2f) * 0.08f);
                float squashY = 1f - (Mathf.Sin(bobTimer * 2f) * 0.08f);

                visualRoot.localScale = new Vector3(squashX, squashY, 1f);
                visualRoot.localPosition = new Vector3(0f, bounceY, 0f);

                // Eye gaze towards movement
                Vector2 eyeOffset = moveInput.normalized * 0.08f;
                if (leftEyeRenderer != null) leftEyeRenderer.transform.localPosition = new Vector3(-0.1f + eyeOffset.x, 0.45f + eyeOffset.y, 0f);
                if (rightEyeRenderer != null) rightEyeRenderer.transform.localPosition = new Vector3(0.1f + eyeOffset.x, 0.45f + eyeOffset.y, 0f);

                // Alternating hand swing
                float handSwing = Mathf.Sin(bobTimer) * 0.2f;
                if (leftHandRenderer != null && !isPunching)
                    leftHandRenderer.transform.localPosition = new Vector3(-0.35f, 0.1f + handSwing, 0f);
                if (rightHandRenderer != null && !isPunching)
                    rightHandRenderer.transform.localPosition = new Vector3(0.35f, 0.1f - handSwing, 0f);
            }
            else
            {
                // Idle breathing animation
                bobTimer += Time.deltaTime * 2.5f;
                float breath = 1f + (Mathf.Sin(bobTimer) * 0.03f);
                visualRoot.localScale = new Vector3(breath, 1f / breath, 1f);
                visualRoot.localPosition = Vector3.zero;

                if (leftEyeRenderer != null) leftEyeRenderer.transform.localPosition = new Vector3(-0.1f, 0.45f, 0f);
                if (rightEyeRenderer != null) rightEyeRenderer.transform.localPosition = new Vector3(0.1f, 0.45f, 0f);
                if (leftHandRenderer != null && !isPunching) leftHandRenderer.transform.localPosition = new Vector3(-0.35f, 0.1f, 0f);
                if (rightHandRenderer != null && !isPunching) rightHandRenderer.transform.localPosition = new Vector3(0.35f, 0.1f, 0f);
            }

            // Punch animation interpolation
            if (isPunching)
            {
                punchProgress += Time.deltaTime * 12f;
                float punchOffset = Mathf.Sin(punchProgress * Mathf.PI) * 0.45f;
                if (rightHandRenderer != null)
                {
                    rightHandRenderer.transform.localPosition = new Vector3(0.35f, 0.1f + punchOffset, 0f);
                }

                if (punchProgress >= 1f)
                {
                    isPunching = false;
                    punchProgress = 0f;
                }
            }
        }

        public void TriggerPunch()
        {
            isPunching = true;
            punchProgress = 0f;
        }

        public void PlayHitEffect()
        {
            StartCoroutine(FlashWhiteRoutine());
        }

        private System.Collections.IEnumerator FlashWhiteRoutine()
        {
            if (headRenderer != null) headRenderer.color = Color.white;
            if (bodyRenderer != null) bodyRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            SetColor(playerColor);
        }

        private void CreateProceduralSpritesIfMissing()
        {
            Sprite circleSprite = CreateCircleSprite(64);
            Sprite pillSprite = CreateCapsuleSprite(32, 64);
            Sprite eyeSprite = CreateCircleSprite(16);

            // Head
            if (headRenderer == null)
            {
                var headObj = new GameObject("Head");
                headObj.transform.SetParent(visualRoot, false);
                headObj.transform.localPosition = new Vector3(0f, 0.45f, 0f);
                headObj.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
                headRenderer = headObj.AddComponent<SpriteRenderer>();
                headRenderer.sprite = circleSprite;
                headRenderer.sortingOrder = 2;
            }

            // Eyes
            if (leftEyeRenderer == null)
            {
                var eyeObj = new GameObject("LeftEye");
                eyeObj.transform.SetParent(visualRoot, false);
                eyeObj.transform.localPosition = new Vector3(-0.1f, 0.45f, 0f);
                eyeObj.transform.localScale = new Vector3(0.18f, 0.24f, 1f);
                leftEyeRenderer = eyeObj.AddComponent<SpriteRenderer>();
                leftEyeRenderer.sprite = eyeSprite;
                leftEyeRenderer.color = Color.black;
                leftEyeRenderer.sortingOrder = 3;
            }

            if (rightEyeRenderer == null)
            {
                var eyeObj = new GameObject("RightEye");
                eyeObj.transform.SetParent(visualRoot, false);
                eyeObj.transform.localPosition = new Vector3(0.1f, 0.45f, 0f);
                eyeObj.transform.localScale = new Vector3(0.18f, 0.24f, 1f);
                rightEyeRenderer = eyeObj.AddComponent<SpriteRenderer>();
                rightEyeRenderer.sprite = eyeSprite;
                rightEyeRenderer.color = Color.black;
                rightEyeRenderer.sortingOrder = 3;
            }

            // Torso / Body
            if (bodyRenderer == null)
            {
                var bodyObj = new GameObject("Torso");
                bodyObj.transform.SetParent(visualRoot, false);
                bodyObj.transform.localPosition = new Vector3(0f, 0.05f, 0f);
                bodyObj.transform.localScale = new Vector3(0.45f, 0.55f, 1f);
                bodyRenderer = bodyObj.AddComponent<SpriteRenderer>();
                bodyRenderer.sprite = pillSprite;
                bodyRenderer.sortingOrder = 1;
            }

            // Hands / Fists
            if (leftHandRenderer == null)
            {
                var handObj = new GameObject("LeftHand");
                handObj.transform.SetParent(visualRoot, false);
                handObj.transform.localPosition = new Vector3(-0.35f, 0.1f, 0f);
                handObj.transform.localScale = new Vector3(0.28f, 0.28f, 1f);
                leftHandRenderer = handObj.AddComponent<SpriteRenderer>();
                leftHandRenderer.sprite = circleSprite;
                leftHandRenderer.sortingOrder = 2;
            }

            if (rightHandRenderer == null)
            {
                var handObj = new GameObject("RightHand");
                handObj.transform.SetParent(visualRoot, false);
                handObj.transform.localPosition = new Vector3(0.35f, 0.1f, 0f);
                handObj.transform.localScale = new Vector3(0.28f, 0.28f, 1f);
                rightHandRenderer = handObj.AddComponent<SpriteRenderer>();
                rightHandRenderer.sprite = circleSprite;
                rightHandRenderer.sortingOrder = 2;
            }

            SetColor(playerColor);
        }

        private Sprite CreateCircleSprite(int resolution)
        {
            Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            float center = resolution * 0.5f;
            float radius = center - 1f;

            Color[] colors = new Color[resolution * resolution];
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    float alpha = Mathf.Clamp01((radius - dist) + 0.5f);
                    colors[y * resolution + x] = new Color(1f, 1f, 1f, alpha);
                }
            }
            tex.SetPixels(colors);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), 100f);
        }

        private Sprite CreateCapsuleSprite(int width, int height)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            Color[] colors = new Color[width * height];
            float radius = width * 0.5f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float cy = Mathf.Clamp(y, radius, height - radius);
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(radius, cy));
                    float alpha = Mathf.Clamp01((radius - dist) + 0.5f);
                    colors[y * width + x] = new Color(1f, 1f, 1f, alpha);
                }
            }
            tex.SetPixels(colors);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
