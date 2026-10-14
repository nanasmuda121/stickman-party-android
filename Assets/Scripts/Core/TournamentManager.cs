using System.Collections.Generic;
using UnityEngine;
using StickmanParty.Minigames;

namespace StickmanParty.Core
{
    public class TournamentManager : MonoBehaviour
    {
        public static TournamentManager Instance { get; private set; }

        public int TotalRounds { get; private set; } = 5;
        public int CurrentRound { get; private set; } = 0;

        private List<MinigameType> minigamePlaylist = new List<MinigameType>();

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

        public void StartNewTournament(int rounds = 5)
        {
            TotalRounds = rounds;
            CurrentRound = 0;
            PlayerManager.Instance?.ResetTournamentScores();

            // Prepare 5-game playlist
            minigamePlaylist.Clear();
            minigamePlaylist.Add(MinigameType.Knockout);
            minigamePlaylist.Add(MinigameType.TankBattle);
            minigamePlaylist.Add(MinigameType.Soccer);
            minigamePlaylist.Add(MinigameType.PaintFloor);
            minigamePlaylist.Add(MinigameType.BombTag);

            // Shuffle playlist
            for (int i = 0; i < minigamePlaylist.Count; i++)
            {
                var temp = minigamePlaylist[i];
                int rand = Random.Range(i, minigamePlaylist.Count);
                minigamePlaylist[i] = minigamePlaylist[rand];
                minigamePlaylist[rand] = temp;
            }

            PlayNextRound();
        }

        public void PlayNextRound()
        {
            CurrentRound++;
            if (CurrentRound > TotalRounds || CurrentRound > minigamePlaylist.Count)
            {
                // Tournament Ended! Show final podium!
                EndTournament();
                return;
            }

            MinigameType nextType = minigamePlaylist[CurrentRound - 1];
            GameManager.Instance?.LoadMinigame(nextType);
        }

        private void EndTournament()
        {
            var ranked = PlayerManager.Instance?.GetRankedPlayersByStars();
            UIManager.Instance?.ShowPodium(ranked);
            AudioManager.Instance?.PlaySound(SoundType.StarWin, 1f);
        }
    }
}
