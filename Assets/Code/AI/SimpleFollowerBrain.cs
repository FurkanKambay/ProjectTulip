using System;
using SaintsField;
using Tulip.Character;
using Tulip.Data;
using UnityEngine;

namespace Tulip.AI
{
    public class SimpleFollowerBrain : MonoBehaviour, ICharacterBrain, IJumperBrain
    {
        public event Action OnJump;
        public event Action OnJumpReleased;

        [Header("References")]
        [SerializeField, Required] Health health;

        [Header("Movement")]
        [SerializeField] float stopDistance;

        [Header("Jump")]
        [SerializeField] float heightThresholdToJump;
        [SerializeField] float jumpCooldown;

        public Vector2? AimPosition { get; private set; }
        public float HorizontalMovement { get; private set; }

        public bool WantsToJump { get; private set; }
        public bool WantsToUse { get; private set; }
        public bool WantsToHook { get; private set; }

        private Transform target;
        private Health targetHealth;
        private float timeSinceLastJump;

        private void Awake()
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetHealth = target.GetComponentInChildren<Health>();
        }

        private void Update()
        {
            if (!health || health.IsDead || !targetHealth || targetHealth.IsDead)
            {
                AimPosition = default;
                HorizontalMovement = default;
                WantsToUse = false;

                OnJumpReleased?.Invoke();
                return;
            }

            timeSinceLastJump += Time.deltaTime;

            AimPosition = targetHealth.transform.position;
            Vector2 distanceToTarget = AimPosition!.Value - (Vector2)transform.position;
            bool withinAttackingRange = distanceToTarget.sqrMagnitude < stopDistance * stopDistance;

            WantsToUse = withinAttackingRange;
            HorizontalMovement = withinAttackingRange ? default : Mathf.Sign(distanceToTarget.x);

            // TODO: some enemies may be able to jump later
            // TryJump(distanceToTarget.y);
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
