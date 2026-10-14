using UnityEngine;
using StickmanParty.Core;
using StickmanParty.Player;

namespace StickmanParty.Minigames.BombTag
{
    public class BombItem : MonoBehaviour
    {
        public StickmanController CurrentHolder { get; private set; }
        private float transferCooldown = 0f;
        private SpriteRenderer bombRenderer;
        private float pulseTimer = 0f;

        private void Awake()
        {
            CreateBombVisual();
        }

        public void AttachToPlayer(StickmanController player)
        {
            CurrentHolder = player;
            transform.SetParent(player.transform, false);
            transform.localPosition = new Vector3(0f, 0.9f, 0f);
            transferCooldown = 1.0f; // Prevent immediate ping-pong tag

            AudioManager.Instance?.PlaySound(SoundType.BombTick, 0.8f);
        }

        private void Update()
        {
            if (transferCooldown > 0f)
            {
                transferCooldown -= Time.deltaTime;
            }

            // Pulsating visual
            pulseTimer += Time.deltaTime * 6f;
            float scale = 0.55f + (Mathf.Sin(pulseTimer) * 0.1f);
            transform.localScale = new Vector3(scale, scale, 1f);
        }

        public bool TryTransferTo(StickmanController newHolder)
        {
            if (transferCooldown > 0f || newHolder == CurrentHolder || !newHolder.IsAlive)
            {
                return false;
            }

            AttachToPlayer(newHolder);
            return true;
        }

        private void CreateBombVisual()
        {
            bombRenderer = gameObject.AddComponent<SpriteRenderer>();
            bombRenderer.sortingOrder = 5;

            int res = 32;
            Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
            float center = res * 0.5f;

            for (int y = 0; y < res; y++)
            {
                for (int x = 0; x < res; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(center, center - 3f));
                    if (d < 10f)
                    {
                        // Black bomb body
                        tex.SetPixel(x, y, new Color(0.12f, 0.12f, 0.12f));
                    }
                    else if (y > res - 8 && Mathf.Abs(x - center) < 3f)
                    {
                        // Yellow/orange spark fuse
                        tex.SetPixel(x, y, Color.red);
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            tex.Apply();
            bombRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
