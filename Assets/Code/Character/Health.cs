using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Character
{
    [SelectionBase]
    public class Health : HealthBase
    {
        private void Update() =>
            remainingInvulnerability = Mathf.Max(0, remainingInvulnerability - Time.deltaTime);

        public override void TakeDamage(float damage, IHealth source)
        {
            if (IsInvulnerable || IsDead)
                return;

            CurrentHealth -= damage;
            LatestDamageSource = source;
            remainingInvulnerability = invulnerabilityDuration;

            Vector3 sourcePosition = source is Health sourceHealth
                ? sourceHealth.transform.position
                : transform.position;

            var damageArgs = new DamageEventArgs(damage, source, this, sourcePosition);
            RaiseOnHurt(damageArgs);

            if (IsAlive)
                return;

            LatestDeathSource = source;
            RaiseOnDie(damageArgs);
            enabled = false;
        }

        public override void Revive(IHealth source = null)
        {
            CurrentHealth = maxHealth;
            enabled = true;
            RaiseOnRevive(source ?? this);
        }

        [ContextMenu("Take 10 Damage")]
        public void TakeDamage() => TakeDamage(10f, this);

        private void OnValidate() => CurrentHealth = currentHealth;

        public override string ToString() => Name;
    }
}
