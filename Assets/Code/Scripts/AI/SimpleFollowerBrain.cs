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

        private float timeSinceLastJump;

        private void Awake()
        {
            health = GetComponent<IHealth>();
            if (!target) target = GameObject.FindGameObjectWithTag("Player").transform;
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

            AimPosition = target.transform.position;
            Vector3 distanceToTarget = AimPosition - transform.position;
            float sqrStopDistance = stopDistance * stopDistance;

            WantsToUse = distanceToTarget.sqrMagnitude < sqrStopDistance;
            HorizontalMovement = WantsToUse ? default : Mathf.Sign(distanceToTarget.x);

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
