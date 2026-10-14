using System.Collections.Generic;
using UnityEngine;
using StickmanParty.Minigames;
using StickmanParty.Minigames.Knockout;
using StickmanParty.Minigames.TankBattle;
using StickmanParty.Minigames.Soccer;
using StickmanParty.Minigames.PaintFloor;
using StickmanParty.Minigames.BombTag;
using StickmanParty.Player;

namespace StickmanParty.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private BaseMinigame currentMinigame;
        private List<StickmanController> playerInstances = new List<StickmanController>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartTournament(int humanCount)
        {
            PlayerManager.Instance?.SetHumanCount(humanCount);
            SpawnPlayersIfMissing();
            TournamentManager.Instance?.StartNewTournament(5);
        }

        public void SpawnPlayersIfMissing()
        {
            if (playerInstances.Count == 4)
            {
                // Already spawned, reset them
                for (int i = 0; i < 4; i++)
                {
                    playerInstances[i].gameObject.SetActive(true);
                    playerInstances[i].Initialize(PlayerManager.Instance.GetPlayer(i));
                }
                return;
            }

            // Clean up old if any
            foreach (var p in playerInstances)
            {
                if (p != null) Destroy(p.gameObject);
            }
            playerInstances.Clear();

            for (int i = 0; i < 4; i++)
            {
                GameObject playerObj = new GameObject($"Stickman_P{i + 1}");
                var controller = playerObj.AddComponent<StickmanController>();
                playerObj.AddComponent<StickmanVisual>();
                playerObj.AddComponent<StickmanAI>();

                controller.Initialize(PlayerManager.Instance.GetPlayer(i));
                playerInstances.Add(controller);
            }
        }

        public void LoadMinigame(MinigameType type)
        {
            // Clean up current minigame if any
            if (currentMinigame != null)
            {
                currentMinigame.CleanUp();
                currentMinigame = null;
            }

            // Reset and revive all 4 players
            foreach (var p in playerInstances)
            {
                p.gameObject.SetActive(true);
                p.IsAlive = true;
                p.CanMove = false;
                p.transform.rotation = Quaternion.identity;
                if (p.TryGetComponent<Rigidbody2D>(out var rb))
                {
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }
            }

            // Instantiate requested minigame
            GameObject gameObj = new GameObject($"Minigame_{type}");
            switch (type)
            {
                case MinigameType.Knockout:
                    currentMinigame = gameObj.AddComponent<KnockoutMinigame>();
                    break;
                case MinigameType.TankBattle:
                    currentMinigame = gameObj.AddComponent<TankBattleMinigame>();
                    break;
                case MinigameType.Soccer:
                    currentMinigame = gameObj.AddComponent<SoccerMinigame>();
                    break;
                case MinigameType.PaintFloor:
                    currentMinigame = gameObj.AddComponent<PaintFloorMinigame>();
                    break;
                case MinigameType.BombTag:
                    currentMinigame = gameObj.AddComponent<BombTagMinigame>();
                    break;
            }

            currentMinigame.Setup(playerInstances);
        }

        public void OnMinigameFinished(int winnerPlayerIndex)
        {
            if (currentMinigame != null)
            {
                currentMinigame.CleanUp();
                currentMinigame = null;
            }

            TournamentManager.Instance?.PlayNextRound();
        }
    }
}
