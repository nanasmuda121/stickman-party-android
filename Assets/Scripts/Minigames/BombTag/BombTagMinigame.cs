using System.Collections.Generic;
using UnityEngine;
using StickmanParty.Core;
using StickmanParty.Player;

namespace StickmanParty.Minigames.BombTag
{
    public class BombTagMinigame : BaseMinigame
    {
        public override MinigameType Type => MinigameType.BombTag;
        public override string MinigameName => "BOMB TAG";
        public override string MinigameDescription => "Oper bom sebelum meledak di tanganmu!";

        public static StickmanController CurrentCarrier => activeBomb != null ? activeBomb.CurrentHolder : null;
        private static BombItem activeBomb;

        [SerializeField] private float fuseDuration = 18f;
        private float currentFuse;
        private float tickInterval = 1f;
        private float nextTickTime = 0f;

        public override void Setup(List<StickmanController> players)
        {
            isSurvivalGame = true;
            gameDuration = 60f;
            currentFuse = fuseDuration;

            // Spawn 4 players spread out in arena
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

            // Create and attach bomb to random player
            GameObject bombObj = new GameObject("BombItem");
            activeBomb = bombObj.AddComponent<BombItem>();
            int randIndex = Random.Range(0, players.Count);
            activeBomb.AttachToPlayer(players[randIndex]);

            base.Setup(players);
        }

        protected override void OnMinigameUpdate()
        {
            if (activeBomb == null || activeBomb.CurrentHolder == null) return;

            currentFuse -= Time.deltaTime;

            // Accelerate tick sounds as fuse runs down
            tickInterval = Mathf.Clamp(currentFuse / fuseDuration * 0.8f, 0.15f, 1f);
            if (Time.time >= nextTickTime)
            {
                nextTickTime = Time.time + tickInterval;
                AudioManager.Instance?.PlaySound(SoundType.BombTick, 0.6f);
            }

            // Check collision between carrier and other players to transfer
            if (activeBomb.CurrentHolder != null)
            {
                Vector2 carrierPos = activeBomb.CurrentHolder.transform.position;
                foreach (var player in activePlayers)
                {
                    if (player.IsAlive && player != activeBomb.CurrentHolder)
                    {
                        if (Vector2.Distance(carrierPos, player.transform.position) < 0.95f)
                        {
                            activeBomb.TryTransferTo(player);
                            break;
                        }
                    }
                }
            }

            // Fuse detonates
            if (currentFuse <= 0f)
            {
                DetonateBomb();
            }
        }

        private void DetonateBomb()
        {
            if (activeBomb != null && activeBomb.CurrentHolder != null)
            {
                var victim = activeBomb.CurrentHolder;
                victim.Eliminate();
                AudioManager.Instance?.PlaySound(SoundType.Explosion, 1f);

                // Check remaining alive players
                int aliveCount = 0;
                StickmanController lastPlayer = null;
                List<StickmanController> aliveList = new List<StickmanController>();

                foreach (var p in activePlayers)
                {
                    if (p.IsAlive)
                    {
                        aliveCount++;
                        lastPlayer = p;
                        aliveList.Add(p);
                    }
                }

                if (aliveCount <= 1)
                {
                    EndMinigame(lastPlayer != null ? (int)lastPlayer.Config.slot : -1);
                }
                else
                {
                    // Pass bomb to another alive survivor and reset fuse
                    currentFuse = 12f;
                    int nextIdx = Random.Range(0, aliveList.Count);
                    activeBomb.AttachToPlayer(aliveList[nextIdx]);
                }
            }
        }

        public override void CleanUp()
        {
            if (activeBomb != null)
            {
                Destroy(activeBomb.gameObject);
                activeBomb = null;
            }
            base.CleanUp();
        }
    }
}
