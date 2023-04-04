using Game.Data.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.CharacterController
{
    public class PlayerJump : MonoBehaviour
    {
        public JumpData data;

        public bool CurrentlyJumping => currentlyJumping;

        [Header("Components")]
        private Rigidbody2D body;
        private GroundChecker ground;

        [Header("Calculations")]
        private float jumpSpeed;
        private float defaultGravityScale;
        private float gravMultiplier;

        [Header("Current State")]
        private Vector2 velocity;
        private float coyoteTimeCounter;
        private float jumpBufferCounter;
        private bool desiredJump;
        private bool pressingJump;
        private bool onGround;
        private bool currentlyJumping;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            ground = GetComponent<GroundChecker>();
            defaultGravityScale = 1f;
        }

        private void Start()
        {
            Player.Input.Actions.Player.Jump.started += OnJump;
            Player.Input.Actions.Player.Jump.canceled += OnJump;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                desiredJump = true;
                pressingJump = true;
            }

            if (context.canceled)
                pressingJump = false;
        }

        private void Update()
        {
            SetGravity();

            onGround = ground.IsGrounded;

            if (data.jumpBuffer > 0)
            {
                // Instead of immediately turning off "desireJump", start counting up...
                // All the while, the DoAJump function will repeatedly be fired off
                if (desiredJump)
                {
                    jumpBufferCounter += Time.deltaTime;

                    if (jumpBufferCounter > data.jumpBuffer)
                    {
                        // If time exceeds the jump buffer, turn off "desireJump"
                        desiredJump = false;
                        jumpBufferCounter = 0;
                    }
                }
            }

            if (!currentlyJumping && !onGround)
                coyoteTimeCounter += Time.deltaTime;
            else
                coyoteTimeCounter = 0;
        }

        private void SetGravity()
        {
            var newGravity = new Vector2(0, -2 * data.jumpHeight / (data.timeToJumpApex * data.timeToJumpApex));
            body.gravityScale = newGravity.y / Physics2D.gravity.y * gravMultiplier;
        }

        private void FixedUpdate()
        {
            velocity = body.velocity;

            if (desiredJump)
            {
                DoAJump();
                body.velocity = velocity;
                // Skip gravity calculations this frame, so currentlyJumping doesn't turn off
                // This makes sure you can't do the coyote time double jump bug
                return;
            }

            CalculateGravity();
        }

        private void CalculateGravity()
        {
            if (body.velocity.y > 0.01f)
            {
                if (onGround)
                {
                    gravMultiplier = defaultGravityScale;
                }
                else if (data.hasVariableJumpHeight)
                {
                    if (pressingJump && currentlyJumping)
                        gravMultiplier = data.upwardGravityMultiplier;
                    else
                        gravMultiplier = data.jumpCutOff;
                }
                else
                {
                    gravMultiplier = data.upwardGravityMultiplier;
                }
            }
            else if (body.velocity.y < -0.01f)
            {
                gravMultiplier = onGround
                    ? defaultGravityScale
                    : data.downwardGravityMultiplier;
            }
            else
            {
                if (onGround)
                    currentlyJumping = false;

                gravMultiplier = defaultGravityScale;
            }

            body.velocity = new Vector2(velocity.x, Mathf.Clamp(velocity.y, -data.maxFallSpeed, 100));
        }

        private void DoAJump()
        {
            if (onGround || (coyoteTimeCounter > 0.03f && coyoteTimeCounter < data.coyoteTime))
            {
                desiredJump = false;
                jumpBufferCounter = 0;
                coyoteTimeCounter = 0;

                jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * body.gravityScale * data.jumpHeight);

                if (velocity.y > 0f)
                    jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
                else if (velocity.y < 0f)
                    jumpSpeed += Mathf.Abs(body.velocity.y);

                velocity.y += jumpSpeed;
                currentlyJumping = true;
            }

            if (data.jumpBuffer == 0)
                desiredJump = false;
        }
    }
}
