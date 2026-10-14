using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StickmanParty.Core;
using StickmanParty.Player;

namespace StickmanParty.Minigames
{
    public enum MinigameType
    {
        Knockout,
        TankBattle,
        Soccer,
        PaintFloor,
        BombTag
    }

    public abstract class BaseMinigame : MonoBehaviour
    {
        public abstract MinigameType Type { get; }
        public abstract string MinigameName { get; }
        public abstract string MinigameDescription { get; }

        [Header("Settings")]
        [SerializeField] protected float gameDuration = 45f;
        [SerializeField] protected bool isSurvivalGame = true; // Elimination vs Score timer

        public float RemainingTime { get; protected set; }
        public bool IsRunning { get; protected set; } = false;
        public bool IsCompleted { get; protected set; } = false;

        protected List<StickmanController> activePlayers = new List<StickmanController>();

        public virtual void Setup(List<StickmanController> players)
        {
            activePlayers = new List<StickmanController>(players);
            RemainingTime = gameDuration;
            IsRunning = false;
            IsCompleted = false;

            foreach (var p in activePlayers)
            {
                p.CanMove = false;
                if (p.TryGetComponent<StickmanAI>(out var ai))
                {
                    ai.SetMinigame(this);
                }
                p.OnEliminated += HandlePlayerElimination;
            }

            StartCoroutine(CountdownSequence());
        }

        protected virtual IEnumerator CountdownSequence()
        {
            UIManager.Instance?.ShowAnnouncement(MinigameName, MinigameDescription);
            yield return new WaitForSeconds(1.5f);

            for (int i = 3; i >= 1; i--)
            {
                UIManager.Instance?.ShowCountdown(i.ToString());
                AudioManager.Instance?.PlaySound(SoundType.CountdownBeep);
                yield return new WaitForSeconds(1f);
            }

            UIManager.Instance?.ShowCountdown("GO!");
            AudioManager.Instance?.PlaySound(SoundType.CountdownGo);

            foreach (var p in activePlayers)
            {
                p.CanMove = true;
            }

            IsRunning = true;
            UIManager.Instance?.ShowTouchControls(activePlayers);
            yield return new WaitForSeconds(0.5f);
            UIManager.Instance?.HideCountdown();
        }

        protected virtual void Update()
        {
            if (!IsRunning || IsCompleted) return;

            RemainingTime -= Time.deltaTime;
            UIManager.Instance?.UpdateTimer(RemainingTime);

            if (RemainingTime <= 0f)
            {
                OnTimeExpired();
            }

            OnMinigameUpdate();
        }

        protected virtual void OnMinigameUpdate() { }

        protected virtual void HandlePlayerElimination(StickmanController eliminatedPlayer)
        {
            if (isSurvivalGame && IsRunning)
            {
                int aliveCount = 0;
                StickmanController lastAlive = null;

                foreach (var p in activePlayers)
                {
                    if (p.IsAlive)
                    {
                        aliveCount++;
                        lastAlive = p;
                    }
                }

                if (aliveCount <= 1)
                {
                    int winnerIndex = lastAlive != null ? (int)lastAlive.Config.slot : -1;
                    EndMinigame(winnerIndex);
                }
            }
        }

        protected virtual void OnTimeExpired()
        {
            EndMinigame(-1);
        }

        public virtual void EndMinigame(int winnerPlayerIndex)
        {
            if (IsCompleted) return;
            IsCompleted = true;
            IsRunning = false;

            foreach (var p in activePlayers)
            {
                p.CanMove = false;
                p.OnEliminated -= HandlePlayerElimination;
            }

            UIManager.Instance?.HideTouchControls();
            AudioManager.Instance?.PlaySound(SoundType.StarWin);

            if (winnerPlayerIndex >= 0)
            {
                PlayerManager.Instance?.AwardStars(winnerPlayerIndex, 3);
                UIManager.Instance?.ShowWinnerBanner(PlayerManager.Instance?.GetPlayer(winnerPlayerIndex));
            }
            else
            {
                UIManager.Instance?.ShowAnnouncement("DRAW!", "Waktu Habis!");
            }

            StartCoroutine(FinishAfterDelayRoutine(winnerPlayerIndex));
        }

        private IEnumerator FinishAfterDelayRoutine(int winnerIndex)
        {
            yield return new WaitForSeconds(3.5f);
            GameManager.Instance?.OnMinigameFinished(winnerIndex);
        }

        public virtual void CleanUp()
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}
