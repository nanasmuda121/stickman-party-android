using System.Collections.Generic;
using UnityEngine;
using StickmanParty.Core;
using StickmanParty.Player;

namespace StickmanParty.Minigames.Soccer
{
    public class SoccerMinigame : BaseMinigame
    {
        public override MinigameType Type => MinigameType.Soccer;
        public override string MinigameName => "STICKMAN SOCCER";
        public override string MinigameDescription => "Giring bola dan cetak gol ke gawang lawan!";

        private int redTeamScore = 0;
        private int blueTeamScore = 0;

        private SoccerBall ball;
        private SoccerGoal leftGoal;
        private SoccerGoal rightGoal;
        private List<GameObject> fieldObjects = new List<GameObject>();

        public override void Setup(List<StickmanController> players)
        {
            isSurvivalGame = false;
            gameDuration = 60f;
            redTeamScore = 0;
            blueTeamScore = 0;

            BuildField();

            // Spawn Ball in Center
            GameObject ballObj = new GameObject("SoccerBall");
            ball = ballObj.AddComponent<SoccerBall>();
            fieldObjects.Add(ballObj);

            // Position 4 players (Team Red on Left, Team Blue on Right)
            Vector3[] spawnPositions = new Vector3[]
            {
                new Vector3(-3.5f, 1.5f, 0f),  // P1: Red (Red Team Attacker)
                new Vector3(3.5f, 1.5f, 0f),   // P2: Blue (Blue Team Attacker)
                new Vector3(4.5f, -1.5f, 0f),  // P3: Green (Blue Team Defender)
                new Vector3(-4.5f, -1.5f, 0f)  // P4: Yellow (Red Team Defender)
            };

            for (int i = 0; i < players.Count; i++)
            {
                players[i].transform.position = spawnPositions[i % spawnPositions.Length];
            }

            base.Setup(players);
            UpdateScoreUI();
        }

        private void BuildField()
        {
            // Pitch borders
            CreateWall(new Vector3(0, 4.2f, 0), new Vector2(15f, 0.5f));
            CreateWall(new Vector3(0, -4.2f, 0), new Vector2(15f, 0.5f));
            CreateWall(new Vector3(-7.5f, 2.7f, 0), new Vector2(0.5f, 3.5f));
            CreateWall(new Vector3(-7.5f, -2.7f, 0), new Vector2(0.5f, 3.5f));
            CreateWall(new Vector3(7.5f, 2.7f, 0), new Vector2(0.5f, 3.5f));
            CreateWall(new Vector3(7.5f, -2.7f, 0), new Vector2(0.5f, 3.5f));

            // Goals
            GameObject leftGoalObj = new GameObject("LeftGoal_Red");
            leftGoalObj.transform.position = new Vector3(-7.5f, 0, 0);
            leftGoal = leftGoalObj.AddComponent<SoccerGoal>();
            leftGoal.TeamId = 0; // Conceding here scores for Blue
            leftGoal.OnGoalScored += HandleGoal;
            fieldObjects.Add(leftGoalObj);

            GameObject rightGoalObj = new GameObject("RightGoal_Blue");
            rightGoalObj.transform.position = new Vector3(7.5f, 0, 0);
            rightGoal = rightGoalObj.AddComponent<SoccerGoal>();
            rightGoal.TeamId = 1; // Conceding here scores for Red
            rightGoal.OnGoalScored += HandleGoal;
            fieldObjects.Add(rightGoalObj);
        }

        private void CreateWall(Vector3 pos, Vector2 size)
        {
            GameObject wall = new GameObject("FieldWall");
            wall.transform.position = pos;
            var col = wall.AddComponent<BoxCollider2D>();
            col.size = size;

            var sr = wall.AddComponent<SpriteRenderer>();
            sr.color = Color.white;

            int w = 16, h = 16;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    tex.SetPixel(x, y, Color.white);
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 16f);
            wall.transform.localScale = new Vector3(size.x, size.y, 1f);

            fieldObjects.Add(wall);
        }

        private void HandleGoal(int concedingTeam)
        {
            if (!IsRunning || IsCompleted) return;

            if (concedingTeam == 0)
            {
                blueTeamScore++;
            }
            else
            {
                redTeamScore++;
            }

            UpdateScoreUI();
            ball.ResetPosition();
        }

        private void UpdateScoreUI()
        {
            UIManager.Instance?.ShowAnnouncement("SOCCER MATCH", $"RED {redTeamScore} - {blueTeamScore} BLUE");
        }

        protected override void OnTimeExpired()
        {
            if (redTeamScore > blueTeamScore)
            {
                // Red Team wins (P1 & P4)
                PlayerManager.Instance?.AwardStars(0, 3);
                PlayerManager.Instance?.AwardStars(3, 3);
                EndMinigame(0);
            }
            else if (blueTeamScore > redTeamScore)
            {
                // Blue Team wins (P2 & P3)
                PlayerManager.Instance?.AwardStars(1, 3);
                PlayerManager.Instance?.AwardStars(2, 3);
                EndMinigame(1);
            }
            else
            {
                // Draw
                EndMinigame(-1);
            }
        }

        public override void CleanUp()
        {
            foreach (var fo in fieldObjects)
            {
                if (fo != null) Destroy(fo);
            }
            base.CleanUp();
        }
    }
}
