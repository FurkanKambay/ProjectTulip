using System;
using Game.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Game.CharacterController
{
    public class PlayerMovement : MonoBehaviour
    {
        public MovementData data;

        [Header("Calculations")]
        private float directionX;
        private Vector2 desiredVelocity;
        private Vector2 velocity;
        private float maxSpeedChange;
        private float acceleration;
        private float deceleration;
        private float turnSpeed;

        [Header("Current State")]
        private bool onGround;
        private bool pressingKey;

        private Rigidbody2D body;
        private SpriteRenderer spriteRenderer;
        private GroundChecker ground;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            ground = GetComponent<GroundChecker>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start() => Player.Input.Actions.Player.MoveX.performed += OnMoveX;

        private void OnMoveX(InputAction.CallbackContext context) => directionX = context.ReadValue<float>();

        private void Update()
        {
            pressingKey = directionX != 0;

            if (pressingKey)
                spriteRenderer.flipX = directionX > 0;

            desiredVelocity = new Vector2(directionX, 0f) * Mathf.Max(data.maxSpeed - data.friction, 0f);
        }

        private void FixedUpdate()
        {
            onGround = ground.IsOnGround;
            velocity = body.velocity;

            if (data.useAcceleration)
                RunWithAcceleration();
            else if (onGround)
                RunWithoutAcceleration();
            else
                RunWithAcceleration();
        }

        private void RunWithAcceleration()
        {
            acceleration = onGround ? data.maxAcceleration : data.maxAirAcceleration;
            deceleration = onGround ? data.maxDeceleration : data.maxAirDeceleration;
            turnSpeed = onGround ? data.maxTurnSpeed : data.maxAirTurnSpeed;

            maxSpeedChange = pressingKey switch
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                true when Mathf.Sign(directionX) != Mathf.Sign(velocity.x) => turnSpeed * Time.deltaTime,
                true => acceleration * Time.deltaTime,
                _ => deceleration * Time.deltaTime
            };

            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            body.velocity = velocity;
        }

        private void RunWithoutAcceleration()
        {
            velocity.x = desiredVelocity.x;
            body.velocity = velocity;
        }

        [Serializable]
        internal class AccelerationOptions
        {
            [Range(0f, 100f)] public float maxAcceleration = 50f;
            [Range(0f, 100f)] public float maxDeceleration = 50f;
            [Range(0f, 100f)] public float maxTurnSpeed = 80f;
        }
    }
}
