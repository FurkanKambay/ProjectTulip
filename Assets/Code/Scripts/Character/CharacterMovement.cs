using System;
using Tulip.Data.Gameplay;
using Tulip.Data;
using UnityEngine;

namespace Tulip.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(GroundChecker))]
    public class CharacterMovement : MonoBehaviour, ICharacterMovement
    {
        public MovementConfig config;

        public Vector2 Input { get; set; }

        public Vector2 Velocity => velocity;
        public Vector2 DesiredVelocity => desiredVelocity;

        [Header("Calculations")]
        private Vector2 desiredVelocity;
        private Vector2 velocity;
        private float maxSpeedChange;
        private float acceleration;
        private float deceleration;
        private float turnSpeed;

        [Header("Current State")]
        private bool isGrounded;
        private bool hasAnyMovement;

        private Rigidbody2D body;
        private SpriteRenderer spriteRenderer;
        private GroundChecker ground;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            ground = GetComponent<GroundChecker>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Update()
        {
            hasAnyMovement = Input.x != 0;

            if (hasAnyMovement)
                spriteRenderer.flipX = Input.x < 0;

            desiredVelocity = Input * Mathf.Max(config.maxSpeed - config.friction, 0f);
        }

        private void FixedUpdate()
        {
            isGrounded = ground.IsGrounded;
            velocity = body.velocity;

            if (config.useAcceleration)
                RunWithAcceleration();
            else if (isGrounded)
                RunWithoutAcceleration();
            else
                RunWithAcceleration();
        }

        private void RunWithAcceleration()
        {
            acceleration = isGrounded ? config.maxAcceleration : config.maxAirAcceleration;
            deceleration = isGrounded ? config.maxDeceleration : config.maxAirDeceleration;
            turnSpeed = isGrounded ? config.maxTurnSpeed : config.maxAirTurnSpeed;

            maxSpeedChange = hasAnyMovement switch
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                true when Mathf.Sign(Input.x) != Mathf.Sign(velocity.x) => turnSpeed * Time.deltaTime,
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
