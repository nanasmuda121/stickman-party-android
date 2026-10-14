using System.Collections.Generic;
using UnityEngine;
using StickmanParty.Core;
using StickmanParty.Player;

namespace StickmanParty.Minigames.PaintFloor
{
    public class PaintFloorMinigame : BaseMinigame
    {
        public override MinigameType Type => MinigameType.PaintFloor;
        public override string MinigameName => "COLOR THE FLOOR";
        public override string MinigameDescription => "Warnai lantai sebanyak-banyaknya sebelum waktu habis!";

        private List<FloorTile> allTiles = new List<FloorTile>();

        public override void Setup(List<StickmanController> players)
        {
            isSurvivalGame = false;
            gameDuration = 45f;

            BuildTileGrid(12, 8);

            // Spawn 4 players at symmetrical positions
            Vector3[] spawns = new Vector3[]
            {
                new Vector3(-4f, 2.5f, 0f),
                new Vector3(4f, 2.5f, 0f),
                new Vector3(-4f, -2.5f, 0f),
                new Vector3(4f, -2.5f, 0f)
            };

            for (int i = 0; i < players.Count; i++)
            {
                players[i].transform.position = spawns[i % spawns.Length];
            }

            base.Setup(players);
        }

        private void BuildTileGrid(int cols, int rows)
        {
            float tileSize = 1.05f;
            float startX = -(cols * tileSize * 0.5f) + (tileSize * 0.5f);
            float startY = -(rows * tileSize * 0.5f) + (tileSize * 0.5f);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Vector3 pos = new Vector3(startX + (c * tileSize), startY + (r * tileSize), 0f);
                    GameObject tileObj = new GameObject($"Tile_{c}_{r}");
                    tileObj.transform.position = pos;
                    var tile = tileObj.AddComponent<FloorTile>();
                    allTiles.Add(tile);
                }
            }
        }

        protected override void OnTimeExpired()
        {
            // Tally tiles per player
            int[] tileCounts = new int[4];
            foreach (var tile in allTiles)
            {
                if (tile.OwnerPlayerIndex >= 0 && tile.OwnerPlayerIndex < 4)
                {
                    tileCounts[tile.OwnerPlayerIndex]++;
                }
            }

            int bestPlayer = 0;
            int maxCount = -1;

            for (int i = 0; i < 4; i++)
            {
                if (tileCounts[i] > maxCount)
                {
                    maxCount = tileCounts[i];
                    bestPlayer = i;
                }
            }

            EndMinigame(bestPlayer);
        }

        public override void CleanUp()
        {
            foreach (var tile in allTiles)
            {
                if (tile != null) Destroy(tile.gameObject);
            }
            base.CleanUp();
        }
    }
}
