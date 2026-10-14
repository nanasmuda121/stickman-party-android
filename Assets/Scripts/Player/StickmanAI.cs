using UnityEngine;
using StickmanParty.Minigames;

namespace StickmanParty.Player
{
    [RequireComponent(typeof(StickmanController))]
    public class StickmanAI : MonoBehaviour
    {
        private StickmanController controller;
        private BaseMinigame currentMinigame;

        private float decisionTimer = 0f;
        private Vector2 desiredDirection = Vector2.zero;
        private float nextActionTime = 0f;

        private void Awake()
        {
            controller = GetComponent<StickmanController>();
        }

        public void SetMinigame(BaseMinigame minigame)
        {
            currentMinigame = minigame;
        }

        private void Update()
        {
            if (controller == null || !controller.IsAlive || !controller.CanMove) return;
            if (controller.Config == null || !controller.Config.isBot) return;

            decisionTimer -= Time.deltaTime;
            if (decisionTimer <= 0f)
            {
                decisionTimer = Random.Range(0.08f, 0.2f);
                CalculateBehavior();
            }

            controller.SetMoveInput(desiredDirection);
        }

        private void CalculateBehavior()
        {
            if (currentMinigame == null)
            {
                // Idle wander
                desiredDirection = Random.insideUnitCircle.normalized;
                return;
            }

            switch (currentMinigame.Type)
            {
                case MinigameType.Knockout:
                    BehaviorKnockout();
                    break;

                case MinigameType.TankBattle:
                    BehaviorTank();
                    break;

                case MinigameType.Soccer:
                    BehaviorSoccer();
                    break;

                case MinigameType.PaintFloor:
                    BehaviorPaintFloor();
                    break;

                case MinigameType.BombTag:
                    BehaviorBombTag();
                    break;
            }
        }

        #region Minigame AI Behaviors
        private void BehaviorKnockout()
        {
            Vector2 myPos = transform.position;
            float distFromCenter = myPos.magnitude;

            // Danger check: If too close to edge (> 4.5f), prioritize running to center
            if (distFromCenter > 3.8f)
            {
                desiredDirection = (-myPos).normalized;
                return;
            }

            // Find closest enemy
            StickmanController closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                Vector2 toEnemy = (Vector2)closestEnemy.transform.position - myPos;
                desiredDirection = toEnemy.normalized;

                // If within punch range, punch/push!
                if (toEnemy.magnitude < 1.4f && Time.time > nextActionTime)
                {
                    controller.TryPerformAction();
                    nextActionTime = Time.time + Random.Range(0.5f, 1.2f);
                }
            }
            else
            {
                desiredDirection = Vector2.zero;
            }
        }

        private void BehaviorTank()
        {
            StickmanController closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                Vector2 toEnemy = (Vector2)closestEnemy.transform.position - (Vector2)transform.position;
                desiredDirection = toEnemy.normalized;

                // Shoot when roughly aligned
                if (Time.time > nextActionTime)
                {
                    controller.TryPerformAction();
                    nextActionTime = Time.time + Random.Range(0.8f, 1.6f);
                }
            }
        }

        private void BehaviorSoccer()
        {
            GameObject ballObj = GameObject.FindWithTag("Ball");
            if (ballObj != null)
            {
                Vector2 ballPos = ballObj.transform.position;
                Vector2 toBall = ballPos - (Vector2)transform.position;

                // Move towards ball
                desiredDirection = toBall.normalized;

                // If close to ball, kick / dash towards goal
                if (toBall.magnitude < 1.2f && Time.time > nextActionTime)
                {
                    controller.TryPerformAction();
                    nextActionTime = Time.time + Random.Range(0.6f, 1.5f);
                }
            }
        }

        private void BehaviorPaintFloor()
        {
            // Move around to paint unvisited areas
            if (desiredDirection == Vector2.zero || Random.value < 0.1f)
            {
                // Wander across the arena grid
                desiredDirection = Random.insideUnitCircle.normalized;
            }
        }

        private void BehaviorBombTag()
        {
            var bombHolder = BombTag.BombTagMinigame.CurrentCarrier;
            if (bombHolder == null) return;

            if (bombHolder == controller)
            {
                // I have the bomb! Chase closest other player to tag them!
                StickmanController target = FindClosestEnemy();
                if (target != null)
                {
                    Vector2 toTarget = (Vector2)target.transform.position - (Vector2)transform.position;
                    desiredDirection = toTarget.normalized;

                    if (toTarget.magnitude < 1.5f && Time.time > nextActionTime)
                    {
                        controller.TryPerformAction();
                        nextActionTime = Time.time + 0.8f;
                    }
                }
            }
            else
            {
                // Run AWAY from the bomb carrier!
                Vector2 fromBomb = (Vector2)transform.position - (Vector2)bombHolder.transform.position;
                desiredDirection = fromBomb.normalized;
            }
        }

        private StickmanController FindClosestEnemy()
        {
            StickmanController closest = null;
            float minDist = float.MaxValue;

            var allPlayers = FindObjectsOfType<StickmanController>();
            foreach (var p in allPlayers)
            {
                if (p == controller || !p.IsAlive) continue;
                float d = Vector2.Distance(transform.position, p.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    closest = p;
                }
            }
            return closest;
        }
        #endregion
    }
}
