using System;
using Tulip.Data;
using UnityEngine;

namespace Tulip.AI
{
    public class SimpleFollowerBrain : MonoBehaviour, IWalkerBrain, IJumperBrain, IWielderBrain
    {
        public event Action OnJump;
        public event Action OnJumpReleased;

        [Header("Movement")]
        [SerializeField] float stopDistance;

        [Header("Jump")]
        [SerializeField] float heightThresholdToJump;
        [SerializeField] float jumpCooldown;

        public float HorizontalMovement { get; private set; }

        public Vector3 AimPosition { get; private set; }
        public bool WantsToUse { get; private set; }

        private IHealth health;
        private Transform target;
        private IHealth targetHealth;

        private float timeSinceLastJump;

        private void Awake()
        {
            health = GetComponent<IHealth>();
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetHealth = target.GetComponent<IHealth>();
        }

        private void Update()
        {
            if (!target) return;

            if (health.IsDead)
            {
                OnJumpReleased?.Invoke();
                return;
            }

            timeSinceLastJump += Time.deltaTime;

            AimPosition = target.position;
            Vector3 distanceToTarget = AimPosition - transform.position;
            bool withinAttackingRange = distanceToTarget.sqrMagnitude < stopDistance * stopDistance;

            WantsToUse = withinAttackingRange && targetHealth.IsAlive;
            HorizontalMovement = withinAttackingRange ? default : Mathf.Sign(distanceToTarget.x);

            TryJump(distanceToTarget.y);
        }

        private void TryJump(float heightDifference)
        {
            if (timeSinceLastJump < jumpCooldown) return;
            if (heightDifference <= heightThresholdToJump) return;

            timeSinceLastJump = 0f;
            OnJump?.Invoke();
        }
    }
}
