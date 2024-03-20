using Tulip.Character;
using Tulip.Data;
using UnityEngine;

namespace Tulip.AI
{
    public class SimpleFollowerBrain : CharacterBrain
    {
        [Header("Movement")]
        [SerializeField] float stopDistance;

        [Header("Jump")]
        [SerializeField] float heightThresholdToJump;
        [SerializeField] private float jumpCooldown;

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

            Vector3 distanceToTarget = target.transform.position - transform.position;
            float sqrStopDistance = stopDistance * stopDistance;

            float movementAmount = distanceToTarget.sqrMagnitude > sqrStopDistance
                ? Mathf.Sign(distanceToTarget.x)
                : default;

            RaiseOnMoveLateral(movementAmount);
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
