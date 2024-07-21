using SaintsField;
using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Gameplay
{
    public class Knockbackable : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] Rigidbody2D body;
        [SerializeField, Required] HealthBase health;

        [Header("Config")]
        [SerializeField] float hurtForceAmount;
        [SerializeField] float deathForceAmount;

        private void HandleHurt(DamageEventArgs damage) => ApplyKnockback(hurtForceAmount, damage.SourcePosition);
        private void HandleDeath(DamageEventArgs death) => ApplyKnockback(deathForceAmount, death.SourcePosition);
        private void HandleRevived(IHealth source) => body.linearVelocity = Vector2.zero;

        private void ApplyKnockback(float forceAmount, Vector3 sourcePosition)
        {
            Vector3 direction = (transform.position - sourcePosition).normalized;
            body.linearVelocity = direction * forceAmount;
        }

        private void OnEnable()
        {
            health.OnHurt += HandleHurt;
            health.OnDie += HandleDeath;
            health.OnRevive += HandleRevived;
        }

        private void OnDisable()
        {
            health.OnHurt -= HandleHurt;
            health.OnDie -= HandleDeath;
            health.OnRevive -= HandleRevived;
        }
    }
}
