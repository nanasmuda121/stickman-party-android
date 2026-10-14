using System.Collections.Generic;
using UnityEngine;
using StickmanParty.Player;

namespace StickmanParty.Minigames.Knockout
{
    public class KnockoutMinigame : BaseMinigame
    {
        public override MinigameType Type => MinigameType.Knockout;
        public override string MinigameName => "LAVA ARENA KNOCKOUT";
        public override string MinigameDescription => "Dorong musuh keluar platform sebelum tereliminasi!";

        private ShrinkingPlatform platform;

        public override void Setup(List<StickmanController> players)
        {
            isSurvivalGame = true;
            gameDuration = 60f;

            // Create circular platform
            GameObject platformObj = new GameObject("ShrinkingPlatform");
            platform = platformObj.AddComponent<ShrinkingPlatform>();

            // Position 4 players radially around the center
            float spawnRadius = 2.8f;
            for (int i = 0; i < players.Count; i++)
            {
                float angle = (i * 90f) * Mathf.Deg2Rad;
                Vector3 spawnPos = new Vector3(Mathf.Cos(angle) * spawnRadius, Mathf.Sin(angle) * spawnRadius, 0f);
                players[i].transform.position = spawnPos;
                players[i].transform.rotation = Quaternion.Euler(0, 0, (i * 90f) + 180f);
            }

            base.Setup(players);
        }

        protected override void OnMinigameUpdate()
        {
            if (platform != null)
            {
                platform.Shrink(Time.deltaTime);

                // Check players boundary
                foreach (var player in activePlayers)
                {
                    if (player.IsAlive && !platform.IsInsidePlatform(player.transform.position))
                    {
                        player.Eliminate();
                    }
                }
            }
        }

        public override void CleanUp()
        {
            if (platform != null)
            {
                Destroy(platform.gameObject);
            }
            base.CleanUp();
        }
    }
}
