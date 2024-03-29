using System;
using Tulip.Data.Gameplay;
using Tulip.Data;
using UnityEngine;

namespace Tulip.Character
{
    [RequireComponent(typeof(IWalkerBrain))]
    [RequireComponent(typeof(GroundChecker))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterMovement : MonoBehaviour, ICharacterMovement
    {
        public MovementConfig config;

        public Vector2 DesiredVelocity { get; private set; }
        public Vector2 Velocity => velocity;

        // Calculations
        private Vector2 velocity;
        private float maxSpeedChange;
        private float acceleration;
        private float deceleration;
        private float turnSpeed;

        // Current State
        private bool isGrounded;
        private bool hasAnyMovement;

        private IWalkerBrain brain;
        private Rigidbody2D body;
        private SpriteRenderer spriteRenderer;
        private GroundChecker ground;

        private void Awake()
        {
            brain = GetComponent<IWalkerBrain>();
            body = GetComponent<Rigidbody2D>();
            ground = GetComponent<GroundChecker>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Update()
        {
            hasAnyMovement = brain.HorizontalMovement != 0;

            if (hasAnyMovement && spriteRenderer)
                spriteRenderer.flipX = brain.HorizontalMovement < 0;

            DesiredVelocity = new Vector2(brain.HorizontalMovement, default) * Mathf.Max(config.maxSpeed - config.friction, 0f);
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
                true when Mathf.Sign(brain.HorizontalMovement) != Mathf.Sign(Velocity.x) => turnSpeed * Time.deltaTime,
                true => acceleration * Time.deltaTime,
                _ => deceleration * Time.deltaTime
            };

            velocity.x = Mathf.MoveTowards(Velocity.x, DesiredVelocity.x, maxSpeedChange);
            body.velocity = Velocity;
        }

        private void RunWithoutAcceleration()
        {
            velocity.x = DesiredVelocity.x;
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
