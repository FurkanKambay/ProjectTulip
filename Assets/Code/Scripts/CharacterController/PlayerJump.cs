using Tulip.Data.Gameplay;
using Tulip.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.CharacterController
{
    public class PlayerJump : MonoBehaviour
    {
        public JumpConfig config;

        public bool IsJumping { get; private set; }

        [Header("Components")]
        private Rigidbody2D body;
        private GroundChecker ground;

        [Header("Calculations")]
        private float jumpSpeed;
        private float defaultGravityScale;
        private float gravityMultiplier;

        [Header("Current State")]
        private Vector2 velocity;

        private float coyoteTimeCounter;
        private float jumpBufferCounter;
        private bool isJumpDesired;
        private bool isPressingJump;
        private bool isGrounded;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            ground = GetComponent<GroundChecker>();
            defaultGravityScale = 1f;
        }

        private void OnEnable()
        {
            InputHelper.Actions.Player.Jump.started += HandleJump;
            InputHelper.Actions.Player.Jump.canceled += HandleJump;
        }

        private void OnDisable()
        {
            InputHelper.Actions.Player.Jump.started -= HandleJump;
            InputHelper.Actions.Player.Jump.canceled -= HandleJump;
        }

        private void Update()
        {
            SetGravity();

            isGrounded = ground.IsGrounded;

            if (config.jumpBuffer > 0)
            {
                // Instead of immediately turning off "desireJump", start counting up...
                // All the while, the DoAJump function will repeatedly be fired off
                if (isJumpDesired)
                {
                    jumpBufferCounter += Time.deltaTime;

                    if (jumpBufferCounter > config.jumpBuffer)
                    {
                        // If time exceeds the jump buffer, turn off "desireJump"
                        isJumpDesired = false;
                        jumpBufferCounter = 0;
                    }
                }
            }

            if (!IsJumping && !isGrounded)
                coyoteTimeCounter += Time.deltaTime;
            else
                coyoteTimeCounter = 0;
        }

        private void FixedUpdate()
        {
            velocity = body.velocity;

            if (isJumpDesired)
            {
                Jump();
                body.velocity = velocity;
                // Skip gravity calculations this frame, so currentlyJumping doesn't turn off
                // This makes sure you can't do the coyote time double jump bug
                return;
            }

            SetVerticalVelocity();
        }

        private void HandleJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                isJumpDesired = true;
                isPressingJump = true;
            }

            if (context.canceled)
                isPressingJump = false;
        }

        private void SetGravity()
        {
            var newGravity = new Vector2(0, -2f * config.jumpHeight / (config.timeToJumpApex * config.timeToJumpApex));
            body.gravityScale = newGravity.y / Physics2D.gravity.y * gravityMultiplier;
        }

        private void SetVerticalVelocity()
        {
            switch (body.velocity.y)
            {
                case > 0.01f when isGrounded:
                    gravityMultiplier = defaultGravityScale;
                    break;
                case > 0.01f when config.hasVariableJumpHeight:
                {
                    if (isPressingJump && IsJumping)
                        gravityMultiplier = config.upwardGravityMultiplier;
                    else
                        gravityMultiplier = config.jumpCutOff;
                    break;
                }
                case > 0.01f:
                    gravityMultiplier = config.upwardGravityMultiplier;
                    break;
                case < -0.01f:
                    gravityMultiplier = isGrounded
                        ? defaultGravityScale
                        : config.downwardGravityMultiplier;
                    break;
                default:
                {
                    if (isGrounded)
                        IsJumping = false;

                    gravityMultiplier = defaultGravityScale;
                    break;
                }
            }

            body.velocity = new Vector2(velocity.x, Mathf.Clamp(velocity.y, -config.maxFallSpeed, 100));
        }

        private void Jump()
        {
            if (isGrounded || (coyoteTimeCounter > 0.03f && coyoteTimeCounter < config.coyoteTime))
            {
                isJumpDesired = false;
                jumpBufferCounter = 0;
                coyoteTimeCounter = 0;

                jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * body.gravityScale * config.jumpHeight);

                if (velocity.y > 0f)
                    jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
                else if (velocity.y < 0f)
                    jumpSpeed += Mathf.Abs(body.velocity.y);

                velocity.y += jumpSpeed;
                IsJumping = true;
            }

            if (config.jumpBuffer == 0)
                isJumpDesired = false;
        }
    }
}
