using System;
using UnityEngine;
using StickmanParty.Core;

namespace StickmanParty.Minigames.Soccer
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SoccerGoal : MonoBehaviour
    {
        public int TeamId; // 0: Red Team Goal, 1: Blue Team Goal
        public event Action<int> OnGoalScored; // passes TeamId that conceded goal

        private void Awake()
        {
            var col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.2f, 3.2f);
            CreateNetVisual();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Ball"))
            {
                AudioManager.Instance?.PlaySound(SoundType.GoalCheer);
                AudioManager.Instance?.PlaySound(SoundType.Whistle);
                OnGoalScored?.Invoke(TeamId);
            }
        }

        private void CreateNetVisual()
        {
            if (GetComponent<SpriteRenderer>() == null)
            {
                var sr = gameObject.AddComponent<SpriteRenderer>();
                sr.color = TeamId == 0 ? new Color(1f, 0.3f, 0.3f, 0.4f) : new Color(0.3f, 0.5f, 1f, 0.4f);
                sr.sortingOrder = 0;

                int w = 16, h = 32;
                Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        tex.SetPixel(x, y, Color.white);
                tex.Apply();

                sr.sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 16f);
                transform.localScale = new Vector3(1.2f, 3.2f, 1f);
            }
        }
    }
}
