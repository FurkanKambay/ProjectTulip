using Tulip.Character;
using Tulip.Data;
using UnityEngine;

namespace Tulip.AI
{
    public class SimpleFollowerBrain : WielderBrain
    {
        [Header("Movement")]
        [SerializeField] float stopDistance;

        [Header("Jump")]
        [SerializeField] float heightThresholdToJump;
        [SerializeField] private float jumpCooldown;

        public override Vector3 FocusPosition { get; protected set; }

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
                RaiseOnMoveLateral(default);
                RaiseOnJumpReleased();
                return;
            }

            timeSinceLastJump += Time.deltaTime;

            FocusPosition = target.transform.position;
            Vector3 distanceToTarget = FocusPosition - transform.position;
            float sqrStopDistance = stopDistance * stopDistance;

            IsUseInProgress = distanceToTarget.sqrMagnitude < sqrStopDistance;
            HorizontalMovement = IsUseInProgress ? default : Mathf.Sign(distanceToTarget.x);

            RaiseOnMoveLateral(HorizontalMovement);
            TryJump(distanceToTarget.y);
        }

        private void TryJump(float heightDifference)
        {
            if (timeSinceLastJump < jumpCooldown) return;
            if (heightDifference <= heightThresholdToJump) return;

            timeSinceLastJump = 0f;
            RaiseOnJump();
        }
    }
}
