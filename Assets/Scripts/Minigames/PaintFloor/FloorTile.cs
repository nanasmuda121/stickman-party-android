using UnityEngine;
using StickmanParty.Core;
using StickmanParty.Player;

namespace StickmanParty.Minigames.PaintFloor
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class FloorTile : MonoBehaviour
    {
        public int OwnerPlayerIndex { get; private set; } = -1;
        private SpriteRenderer sr;
        private static readonly Color neutralColor = new Color(0.85f, 0.88f, 0.92f);

        private void Awake()
        {
            var col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.95f, 0.95f);

            sr = GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = gameObject.AddComponent<SpriteRenderer>();
                sr.sortingOrder = -1; // Floor layer

                int res = 16;
                Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
                for (int y = 0; y < res; y++)
                    for (int x = 0; x < res; x++)
                        tex.SetPixel(x, y, Color.white);
                tex.Apply();
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), 16f);
            }
            sr.color = neutralColor;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<StickmanController>(out var player) && player.IsAlive)
            {
                int playerIdx = (int)player.Config.slot;
                if (OwnerPlayerIndex != playerIdx)
                {
                    OwnerPlayerIndex = playerIdx;
                    sr.color = player.Config.playerColor;
                    AudioManager.Instance?.PlaySound(SoundType.TilePaint, 0.2f);
                }
            }
        }
    }
}
