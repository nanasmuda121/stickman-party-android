using UnityEngine;
using StickmanParty.Core;
using StickmanParty.Player;

namespace StickmanParty.Minigames.TankBattle
{
    [RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
    public class TankBullet : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private int maxBounces = 3;

        private int currentBounces = 0;
        private Rigidbody2D rb;
        private StickmanController shooter;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var col = GetComponent<CircleCollider2D>();
            col.radius = 0.2f;

            CreateVisualIfMissing();
        }

        public void Launch(Vector2 direction, StickmanController owner)
        {
            shooter = owner;
            rb.velocity = direction.normalized * speed;
            Destroy(gameObject, 6f); // Safety despawn
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Hit a player
            if (collision.gameObject.TryGetComponent<StickmanController>(out var player))
            {
                player.Eliminate();
                Explode();
                return;
            }

            // Hit destructible crate
            if (collision.gameObject.TryGetComponent<DestructibleCrate>(out var crate))
            {
                crate.DestroyCrate();
                Explode();
                return;
            }

            // Bounce off walls
            currentBounces++;
            if (currentBounces >= maxBounces)
            {
                Explode();
            }
            else
            {
                Vector2 normal = collision.contacts[0].normal;
                rb.velocity = Vector2.Reflect(rb.velocity, normal).normalized * speed;
                AudioManager.Instance?.PlaySound(SoundType.Punch, 0.4f);
            }
        }

        private void Explode()
        {
            AudioManager.Instance?.PlaySound(SoundType.Explosion, 0.6f);
            Destroy(gameObject);
        }

        private void CreateVisualIfMissing()
        {
            if (GetComponent<SpriteRenderer>() == null)
            {
                var sr = gameObject.AddComponent<SpriteRenderer>();
                sr.color = Color.white;
                sr.sortingOrder = 5;

                // Simple white circle
                int res = 16;
                Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
                for (int y = 0; y < res; y++)
                {
                    for (int x = 0; x < res; x++)
                    {
                        float d = Vector2.Distance(new Vector2(x, y), new Vector2(res * 0.5f, res * 0.5f));
                        tex.SetPixel(x, y, d < (res * 0.45f) ? Color.yellow : Color.clear);
                    }
                }
                tex.Apply();
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), 100f);
                transform.localScale = new Vector3(0.35f, 0.35f, 1f);
            }
        }
    }
}
