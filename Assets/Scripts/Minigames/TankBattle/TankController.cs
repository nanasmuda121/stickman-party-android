using UnityEngine;
using StickmanParty.Core;
using StickmanParty.Player;

namespace StickmanParty.Minigames.TankBattle
{
    public class TankController : MonoBehaviour
    {
        [SerializeField] private float shootCooldown = 1.0f;
        private float lastShootTime = -10f;
        private StickmanController stickman;
        private Transform cannonTransform;

        private void Awake()
        {
            stickman = GetComponent<StickmanController>();
            SetupTankVisual();
        }

        private void OnEnable()
        {
            if (stickman != null)
            {
                stickman.OnActionTriggered += HandleShoot;
            }
        }

        private void OnDisable()
        {
            if (stickman != null)
            {
                stickman.OnActionTriggered -= HandleShoot;
            }
        }

        private void SetupTankVisual()
        {
            // Create a little cannon barrel sticking out of the stickman
            GameObject barrel = new GameObject("CannonBarrel");
            barrel.transform.SetParent(transform, false);
            barrel.transform.localPosition = new Vector3(0f, 0.45f, 0f);

            var sr = barrel.AddComponent<SpriteRenderer>();
            sr.color = Color.gray;
            sr.sortingOrder = 4;

            int w = 8, h = 16;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    tex.SetPixel(x, y, Color.black);
                }
            }
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0f), 100f);
            cannonTransform = barrel.transform;
        }

        private void HandleShoot(StickmanController player)
        {
            if (Time.time < lastShootTime + shootCooldown) return;
            lastShootTime = Time.time;

            Vector2 shootDir = stickman.GetFacingDirection();
            Vector3 spawnPos = transform.position + (Vector3)(shootDir * 0.7f);

            GameObject bulletObj = new GameObject("TankBullet");
            bulletObj.transform.position = spawnPos;
            var bullet = bulletObj.AddComponent<TankBullet>();
            bullet.Launch(shootDir, stickman);

            AudioManager.Instance?.PlaySound(SoundType.TankShoot, 0.8f);
        }
    }
}
