using UnityEngine;
using StickmanParty.Core;
using StickmanParty.Player;

namespace StickmanParty.Minigames.Soccer
{
    [RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
    public class SoccerBall : MonoBehaviour
    {
        private Rigidbody2D rb;
        private CircleCollider2D col;

        private void Awake()
        {
            tag = "Ball";
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<CircleCollider2D>();

            rb.gravityScale = 0f;
            rb.drag = 1.2f;
            rb.angularDrag = 1.5f;
            col.radius = 0.4f;

            PhysicsMaterial2D bounceMat = new PhysicsMaterial2D("BallMaterial");
            bounceMat.bounciness = 0.85f;
            bounceMat.friction = 0.2f;
            col.sharedMaterial = bounceMat;

            CreateBallVisual();
        }

        public void Kick(Vector2 direction, float force)
        {
            rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
            AudioManager.Instance?.PlaySound(SoundType.Punch, 0.7f);
        }

        public void ResetPosition()
        {
            transform.position = Vector3.zero;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<StickmanController>(out var player))
            {
                Vector2 kickDir = (transform.position - player.transform.position).normalized;
                Kick(kickDir, 5f);
            }
        }

        private void CreateBallVisual()
        {
            if (GetComponent<SpriteRenderer>() == null)
            {
                var sr = gameObject.AddComponent<SpriteRenderer>();
                sr.sortingOrder = 3;

                int res = 32;
                Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
                float center = res * 0.5f;

                for (int y = 0; y < res; y++)
                {
                    for (int x = 0; x < res; x++)
                    {
                        float d = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                        if (d < center - 1f)
                        {
                            // Soccer ball checker pattern
                            bool isBlack = ((x / 6 + y / 6) % 2 == 0);
                            tex.SetPixel(x, y, isBlack ? Color.black : Color.white);
                        }
                        else
                        {
                            tex.SetPixel(x, y, Color.clear);
                        }
                    }
                }
                tex.Apply();
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), 100f);
                transform.localScale = new Vector3(0.8f, 0.8f, 1f);
            }
        }
    }
}
