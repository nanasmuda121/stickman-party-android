using System.Collections.Generic;
using UnityEngine;
using StickmanParty.Player;

namespace StickmanParty.Minigames.TankBattle
{
    public class TankBattleMinigame : BaseMinigame
    {
        public override MinigameType Type => MinigameType.TankBattle;
        public override string MinigameName => "TANK BATTLE";
        public override string MinigameDescription => "Tembak peluru pantul dan hancurkan tank lawan!";

        private List<GameObject> spawnedObstacles = new List<GameObject>();
        private List<TankController> tankControllers = new List<TankController>();

        public override void Setup(List<StickmanController> players)
        {
            isSurvivalGame = true;
            gameDuration = 60f;

            BuildTankArena();

            // Spawn 4 players at corners
            Vector3[] cornerSpawns = new Vector3[]
            {
                new Vector3(-5.5f, 3.2f, 0f),
                new Vector3(5.5f, 3.2f, 0f),
                new Vector3(-5.5f, -3.2f, 0f),
                new Vector3(5.5f, -3.2f, 0f)
            };

            for (int i = 0; i < players.Count; i++)
            {
                players[i].transform.position = cornerSpawns[i % cornerSpawns.Length];
                var tc = players[i].gameObject.AddComponent<TankController>();
                tankControllers.Add(tc);
            }

            base.Setup(players);
        }

        private void BuildTankArena()
        {
            // Perimeter walls
            CreateWall(new Vector3(0, 4.5f, 0), new Vector2(14f, 0.6f));
            CreateWall(new Vector3(0, -4.5f, 0), new Vector2(14f, 0.6f));
            CreateWall(new Vector3(-7f, 0, 0), new Vector2(0.6f, 9.6f));
            CreateWall(new Vector3(7f, 0, 0), new Vector2(0.6f, 9.6f));

            // Interior maze blocks
            CreateWall(new Vector3(-2.5f, 1.5f, 0), new Vector2(2f, 0.6f));
            CreateWall(new Vector3(2.5f, -1.5f, 0), new Vector2(2f, 0.6f));
            CreateWall(new Vector3(0, 0, 0), new Vector2(1.2f, 1.2f));

            // Destructible crates
            CreateCrate(new Vector3(-2.5f, -1.5f, 0));
            CreateCrate(new Vector3(2.5f, 1.5f, 0));
            CreateCrate(new Vector3(0, 2f, 0));
            CreateCrate(new Vector3(0, -2f, 0));
        }

        private void CreateWall(Vector3 pos, Vector2 size)
        {
            GameObject wall = new GameObject("Wall");
            wall.transform.position = pos;
            var col = wall.AddComponent<BoxCollider2D>();
            col.size = size;

            var sr = wall.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.2f, 0.22f, 0.28f);

            int w = 16, h = 16;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    tex.SetPixel(x, y, Color.white);
            tex.Apply();

            sr.sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 16f);
            wall.transform.localScale = new Vector3(size.x, size.y, 1f);

            spawnedObstacles.Add(wall);
        }

        private void CreateCrate(Vector3 pos)
        {
            GameObject crate = new GameObject("Crate");
            crate.transform.position = pos;
            crate.AddComponent<DestructibleCrate>();
            spawnedObstacles.Add(crate);
        }

        public override void CleanUp()
        {
            foreach (var tc in tankControllers)
            {
                if (tc != null) Destroy(tc);
            }
            foreach (var obs in spawnedObstacles)
            {
                if (obs != null) Destroy(obs);
            }
            base.CleanUp();
        }
    }
}
