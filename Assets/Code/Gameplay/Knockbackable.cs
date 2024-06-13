using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Gameplay
{
    public class Knockbackable : MonoBehaviour
    {
        [SerializeField] float hurtForceAmount;
        [SerializeField] float deathForceAmount;

        private Health health;
        private Rigidbody2D body;

        private void Awake()
        {
            health = GetComponent<Health>();
            body = GetComponent<Rigidbody2D>();
        }

        private void HandleHurt(DamageEventArgs damage) => ApplyKnockback(hurtForceAmount, damage.SourcePosition);
        private void HandleDeath(DamageEventArgs death) => ApplyKnockback(deathForceAmount, death.SourcePosition);
        private void HandleRevived(IHealth source) => body.velocity = Vector2.zero;

        private void ApplyKnockback(float forceAmount, Vector3 sourcePosition)
        {
            Vector3 direction = (transform.position - sourcePosition).normalized;
            body.velocity = direction * forceAmount;
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
