using System;
using UnityEngine;
using StickmanParty.Core;

namespace StickmanParty.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class StickmanController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 6.5f;
        [SerializeField] private float punchForce = 12f;
        [SerializeField] private float dashCooldown = 0.6f;

        [Header("References")]
        public PlayerConfig Config { get; private set; }
        public StickmanVisual Visual { get; private set; }
        private Rigidbody2D rb;
        private CircleCollider2D col;

        public bool IsAlive { get; set; } = true;
        public bool IsStunned => stunTimer > 0f;
        public bool CanMove { get; set; } = true;
        public VirtualJoystick Joystick { get; set; }

        private Vector2 moveInput = Vector2.zero;
        private Vector2 facingDirection = Vector2.up;
        private float stunTimer = 0f;
        private float lastActionTime = -10f;

        public event Action<StickmanController> OnActionTriggered;
        public event Action<StickmanController> OnEliminated;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<CircleCollider2D>();
            Visual = GetComponent<StickmanVisual>();

            rb.gravityScale = 0f;
            rb.drag = 6f;
            rb.freezeRotation = true;
            col.radius = 0.45f;
        }

        public void Initialize(PlayerConfig config)
        {
            Config = config;
            if (Visual != null)
            {
                Visual.SetColor(config.playerColor);
            }
            IsAlive = true;
            CanMove = true;
            stunTimer = 0f;
        }

        public void SetMoveInput(Vector2 input)
        {
            if (!IsAlive || IsStunned || !CanMove)
            {
                moveInput = Vector2.zero;
                return;
            }
            moveInput = Vector2.ClampMagnitude(input, 1f);
            if (moveInput.sqrMagnitude > 0.05f)
            {
                facingDirection = moveInput.normalized;
            }
        }

        private void Update()
        {
            if (!IsAlive) return;

            // Handle Stun
            if (stunTimer > 0f)
            {
                stunTimer -= Time.deltaTime;
                moveInput = Vector2.zero;
            }

            // Read Human Keyboard & Mobile Touch Inputs
            if (Config != null && !Config.isBot && CanMove && !IsStunned)
            {
                float h = 0f;
                float v = 0f;

                // 1. Mobile Virtual Joystick
                if (Joystick != null && Joystick.InputVector.sqrMagnitude > 0.01f)
                {
                    h = Joystick.InputVector.x;
                    v = Joystick.InputVector.y;
                }
                else
                {
                    // 2. Keyboard fallback
                    if (Input.GetKey(Config.keyLeft)) h -= 1f;
                    if (Input.GetKey(Config.keyRight)) h += 1f;
                    if (Input.GetKey(Config.keyDown)) v -= 1f;
                    if (Input.GetKey(Config.keyUp)) v += 1f;
                }

                SetMoveInput(new Vector2(h, v));

                if (Input.GetKeyDown(Config.keyAction))
                {
                    TryPerformAction();
                }
            }

            // Visual update
            if (Visual != null)
            {
                Visual.UpdateAnimation(moveInput, rb.velocity.magnitude, moveInput.sqrMagnitude > 0.05f);
            }
        }

        private void FixedUpdate()
        {
            if (!IsAlive || IsStunned || !CanMove) return;

            if (moveInput.sqrMagnitude > 0.01f)
            {
                rb.AddForce(moveInput * (moveSpeed * 18f), ForceMode2D.Force);
            }

            // Rotate towards moving direction
            if (facingDirection.sqrMagnitude > 0.01f)
            {
                float targetAngle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, targetAngle), Time.fixedDeltaTime * 15f);
            }
        }

        public bool TryPerformAction()
        {
            if (!IsAlive || IsStunned || !CanMove) return false;
            if (Time.time < lastActionTime + dashCooldown) return false;

            lastActionTime = Time.time;

            if (Visual != null)
            {
                Visual.TriggerPunch();
            }

            // Perform punch / dash push in facing direction
            Vector2 forward = facingDirection;
            rb.AddForce(forward * (punchForce * 0.8f), ForceMode2D.Impulse);

            AudioManager.Instance?.PlaySound(SoundType.Punch, 0.7f);

            // Check hit against nearby enemies
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position + (Vector3)(forward * 0.6f), 0.65f);
            foreach (var hit in hits)
            {
                if (hit.gameObject != gameObject && hit.TryGetComponent<StickmanController>(out var otherPlayer))
                {
                    otherPlayer.ApplyKnockback(forward * punchForce, 0.35f);
                }
            }

            OnActionTriggered?.Invoke(this);
            return true;
        }

        public void ApplyKnockback(Vector2 force, float stunDuration = 0.25f)
        {
            if (!IsAlive) return;

            rb.velocity = Vector2.zero;
            rb.AddForce(force, ForceMode2D.Impulse);
            stunTimer = stunDuration;

            if (Visual != null)
            {
                Visual.PlayHitEffect();
            }

            AudioManager.Instance?.PlaySound(SoundType.Punch, 0.9f);
        }

        public void Eliminate()
        {
            if (!IsAlive) return;
            IsAlive = false;
            CanMove = false;
            rb.velocity = Vector2.zero;
            gameObject.SetActive(false);

            AudioManager.Instance?.PlaySound(SoundType.Explosion, 0.8f);
            OnEliminated?.Invoke(this);
        }

        public Vector2 GetFacingDirection()
        {
            return facingDirection;
        }
    }
}
