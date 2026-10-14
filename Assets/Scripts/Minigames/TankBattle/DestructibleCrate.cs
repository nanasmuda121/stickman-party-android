using UnityEngine;
using StickmanParty.Core;

namespace StickmanParty.Minigames.TankBattle
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DestructibleCrate : MonoBehaviour
    {
        private void Awake()
        {
            var col = GetComponent<BoxCollider2D>();
            col.size = new Vector2(1f, 1f);
            CreateCrateVisual();
        }

        public void DestroyCrate()
        {
            AudioManager.Instance?.PlaySound(SoundType.Explosion, 0.4f);
            Destroy(gameObject);
        }

        private void CreateCrateVisual()
        {
            if (GetComponent<SpriteRenderer>() == null)
            {
                var sr = gameObject.AddComponent<SpriteRenderer>();
                sr.color = new Color(0.72f, 0.45f, 0.2f); // Wood crate brown
                sr.sortingOrder = 1;

                int res = 32;
                Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
                Color border = new Color(0.45f, 0.28f, 0.12f);
                Color fill = new Color(0.72f, 0.45f, 0.2f);

                for (int y = 0; y < res; y++)
                {
                    for (int x = 0; x < res; x++)
                    {
                        bool isBorder = (x < 2 || x >= res - 2 || y < 2 || y >= res - 2 || x == y || x == (res - 1 - y));
                        tex.SetPixel(x, y, isBorder ? border : fill);
                    }
                }
                tex.Apply();
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), 100f);
            }
        }
    }
}
