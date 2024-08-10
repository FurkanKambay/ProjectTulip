using SaintsField;
using Tulip.Character;
using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.AI
{
    public class TreantAI : MonoBehaviour, IWielderBrain
    {
        [Header("References")]
        [SerializeField, Required] Health health;

        [Header("Movement")]
        [SerializeField] Vector2 attackDistance;

        public Vector2? AimPosition { get; private set; }
        public bool WantsToUse { get; private set; }

        private Transform target;
        private Health targetHealth;

        private void OnEnable() => health.OnHurt += HandleHurt;
        private void OnDisable() => health.OnHurt -= HandleHurt;

        private void HandleHurt(HealthChangeEventArgs damage)
        {
            if (target)
                return;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetHealth = target.GetComponentInChildren<Health>();
        }

        private void Update()
        {
            if (!health || health.IsDead || !targetHealth || targetHealth.IsDead)
            {
                AimPosition = default;
                WantsToUse = false;
                return;
            }

            AimPosition = targetHealth.transform.position;
            Vector2 targetVector = AimPosition!.Value - (Vector2)transform.position;

            bool reachedX = Mathf.Abs(targetVector.x) < attackDistance.x;
            bool reachedY = Mathf.Abs(targetVector.y) < attackDistance.y;
            WantsToUse = reachedX && reachedY;
        }
    }
}
