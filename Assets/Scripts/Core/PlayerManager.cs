using System.Collections.Generic;
using UnityEngine;

namespace StickmanParty.Core
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }

        [SerializeField] private List<PlayerConfig> players = new List<PlayerConfig>();
        public int HumanPlayerCount { get; private set; } = 1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeDefaultPlayers();
        }

        private void InitializeDefaultPlayers()
        {
            players.Clear();
            players.Add(PlayerConfig.GetDefault(PlayerSlot.Player1));
            players.Add(PlayerConfig.GetDefault(PlayerSlot.Player2));
            players.Add(PlayerConfig.GetDefault(PlayerSlot.Player3));
            players.Add(PlayerConfig.GetDefault(PlayerSlot.Player4));

            SetHumanCount(1); // Default: 1 Human vs 3 Bots
        }

        public void SetHumanCount(int count)
        {
            HumanPlayerCount = Mathf.Clamp(count, 1, 4);
            for (int i = 0; i < players.Count; i++)
            {
                players[i].isBot = (i >= HumanPlayerCount);
            }
        }

        public PlayerConfig GetPlayer(int index)
        {
            if (index >= 0 && index < players.Count)
                return players[index];
            return null;
        }

        public List<PlayerConfig> GetAllPlayers()
        {
            return players;
        }

        public void ResetTournamentScores()
        {
            foreach (var p in players)
            {
                p.currentStars = 0;
                p.roundScore = 0;
            }
        }

        public void ResetRoundScores()
        {
            foreach (var p in players)
            {
                p.roundScore = 0;
            }
        }

        public void AwardStars(int playerIndex, int stars)
        {
            if (playerIndex >= 0 && playerIndex < players.Count)
            {
                players[playerIndex].currentStars += stars;
            }
        }

        public List<PlayerConfig> GetRankedPlayersByStars()
        {
            var sorted = new List<PlayerConfig>(players);
            sorted.Sort((a, b) => b.currentStars.CompareTo(a.currentStars));
            return sorted;
        }
    }
}
