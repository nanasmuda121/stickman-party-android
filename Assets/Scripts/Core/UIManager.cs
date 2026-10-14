using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StickmanParty.Core;
using StickmanParty.Player;

namespace StickmanParty.Core
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private Canvas mainCanvas;
        private Text countdownText;
        private Text timerText;
        private Text announcementTitleText;
        private Text announcementDescText;
        private GameObject announcementPanel;

        private GameObject mainMenuPanel;
        private GameObject scoreboardPanel;
        private GameObject podiumPanel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            BuildProceduralUI();
        }

        private void BuildProceduralUI()
        {
            // 1. Create Canvas
            GameObject canvasObj = new GameObject("MainGameCanvas");
            canvasObj.transform.SetParent(transform);
            mainCanvas = canvasObj.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>();

            // 2. Timer Text (Top Center)
            timerText = CreateText(canvasObj.transform, "TimerText", "45", 42, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -50f));

            // 3. Countdown Text (Center)
            countdownText = CreateText(canvasObj.transform, "CountdownText", "", 96, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);
            countdownText.color = Color.yellow;

            // 4. Announcement Banner (Center Box)
            announcementPanel = CreatePanel(canvasObj.transform, "AnnouncementPanel", new Vector2(700, 180), new Color(0f, 0f, 0f, 0.75f));
            announcementTitleText = CreateText(announcementPanel.transform, "Title", "STICKMAN PARTY", 38, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f), Vector2.zero);
            announcementTitleText.color = Color.yellow;
            announcementDescText = CreateText(announcementPanel.transform, "Desc", "Kumpulkan Bintang Terbanyak!", 24, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.35f), new Vector2(0.5f, 0.35f), Vector2.zero);
            announcementPanel.SetActive(false);

            // 5. Main Menu Panel
            BuildMainMenu(canvasObj.transform);

            // 6. Podium Panel
            BuildPodiumPanel(canvasObj.transform);
        }

        private void BuildMainMenu(Transform parent)
        {
            mainMenuPanel = CreatePanel(parent, "MainMenuPanel", new Vector2(600, 450), new Color(0.12f, 0.15f, 0.22f, 0.95f));
            CreateText(mainMenuPanel.transform, "Title", "STICKMAN PARTY", 48, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.85f), Vector2.zero, Color.yellow);
            CreateText(mainMenuPanel.transform, "Subtitle", "Pilih Jumlah Pemain Lokal (1 - 4)", 22, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f), Vector2.zero);

            // Buttons: 1P, 2P, 3P, 4P
            for (int i = 1; i <= 4; i++)
            {
                int count = i;
                CreateButton(mainMenuPanel.transform, $"{count}P Button", $"{count} PEMAIN", new Vector2(0f, 50f - ((count - 1) * 65f)), () =>
                {
                    AudioManager.Instance?.PlaySound(SoundType.ButtonClick);
                    mainMenuPanel.SetActive(false);
                    GameManager.Instance?.StartTournament(count);
                });
            }
        }

        private void BuildPodiumPanel(Transform parent)
        {
            podiumPanel = CreatePanel(parent, "PodiumPanel", new Vector2(750, 500), new Color(0.08f, 0.1f, 0.16f, 0.95f));
            CreateText(podiumPanel.transform, "PodiumTitle", "PODIUM JUARA!", 52, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.85f), Vector2.zero, Color.yellow);

            CreateButton(podiumPanel.transform, "RestartBtn", "MAIN LAGI", new Vector2(0f, -180f), () =>
            {
                AudioManager.Instance?.PlaySound(SoundType.ButtonClick);
                podiumPanel.SetActive(false);
                mainMenuPanel.SetActive(true);
            });

            podiumPanel.SetActive(false);
        }

        #region Public UI Controls
        public void ShowMainMenu()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (podiumPanel != null) podiumPanel.SetActive(false);
            if (announcementPanel != null) announcementPanel.SetActive(false);
        }

        public void ShowCountdown(string text)
        {
            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(true);
                countdownText.text = text;
            }
        }

        public void HideCountdown()
        {
            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(false);
            }
        }

        public void UpdateTimer(float seconds)
        {
            if (timerText != null)
            {
                timerText.text = Mathf.CeilToInt(Mathf.Max(0, seconds)).ToString();
            }
        }

        public void ShowAnnouncement(string title, string desc)
        {
            if (announcementPanel != null)
            {
                announcementPanel.SetActive(true);
                announcementTitleText.text = title;
                announcementDescText.text = desc;
            }
        }

        public void HideAnnouncement()
        {
            if (announcementPanel != null)
            {
                announcementPanel.SetActive(false);
            }
        }

        public void ShowWinnerBanner(PlayerConfig winner)
        {
            if (winner != null)
            {
                ShowAnnouncement($"PEMENANG: {winner.playerName}!", "+3 BINTANG!");
                announcementTitleText.color = winner.playerColor;
            }
            else
            {
                ShowAnnouncement("SERI!", "Tidak ada pemenang!");
                announcementTitleText.color = Color.white;
            }
        }

        public void ShowPodium(List<PlayerConfig> rankedPlayers)
        {
            if (podiumPanel != null)
            {
                podiumPanel.SetActive(true);
                // Create or update podium standings text
                for (int i = 0; i < rankedPlayers.Count; i++)
                {
                    var p = rankedPlayers[i];
                    string rankStr = $"{i + 1}. {p.playerName} : {p.currentStars} BINTANG";
                    var txt = CreateText(podiumPanel.transform, $"Rank_{i}", rankStr, 28, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.65f - (i * 0.12f)), new Vector2(0.5f, 0.65f - (i * 0.12f)), Vector2.zero);
                    txt.color = p.playerColor;
                }
            }
        }
        #endregion

        #region UI Helpers
        private GameObject CreatePanel(Transform parent, string name, Vector2 size, Color color)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            var img = obj.AddComponent<Image>();
            img.color = color;
            return obj;
        }

        private Text CreateText(Transform parent, string name, string content, int fontSize, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Color? color = null)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = new Vector2(600, 80);

            var text = obj.AddComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color ?? Color.white;
            return text;
        }

        private Button CreateButton(Transform parent, string name, string label, Vector2 pos, UnityEngine.Events.UnityAction onClick)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            var rect = btnObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(260, 50);
            rect.anchoredPosition = pos;

            var img = btnObj.AddComponent<Image>();
            img.color = new Color(0.25f, 0.35f, 0.55f);

            var btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(onClick);

            CreateText(btnObj.transform, "BtnText", label, 22, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);
            return btn;
        }
        #endregion
    }
}
