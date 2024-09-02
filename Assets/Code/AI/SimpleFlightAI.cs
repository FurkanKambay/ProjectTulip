using SaintsField;
using Tulip.Character;
using Tulip.Data;
using UnityEngine;

namespace Tulip.AI
{
    public class SimpleFlightAI : MonoBehaviour, IFlightBrain
    {
        [Header("References")]
        [SerializeField, Required] Health health;

        [Header("Movement")]
        [SerializeField] Vector2 stopDistance;

        public float HorizontalMovement { get; private set; }
        public float VerticalMovement { get; private set; }
        public Vector2? AimPosition { get; private set; }
        public bool WantsToUse { get; private set; }
        public bool WantsToHook { get; private set; }

        private Transform target;
        private Health targetHealth;

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
                VerticalMovement = default;
                WantsToUse = false;
                return;
            }

            AimPosition = targetHealth.transform.position;
            Vector2 targetVector = AimPosition!.Value - (Vector2)transform.position;

            bool reachedX = Mathf.Abs(targetVector.x) < stopDistance.x;
            bool reachedY = Mathf.Abs(targetVector.y) < stopDistance.y;

            HorizontalMovement = reachedX ? default : targetVector.normalized.x;
            VerticalMovement = reachedY ? default : targetVector.normalized.y;
            WantsToUse = reachedX && reachedY;
        }
    }
}
