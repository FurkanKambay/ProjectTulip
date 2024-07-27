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

        public override void Damage(float amount, IHealth source)
        {
            if (IsInvulnerable || IsDead)
                return;

            CurrentHealth -= amount;
            LatestDamageSource = source;
            remainingInvulnerability = invulnerabilityDuration;

            Vector3 sourcePosition = source is Health sourceHealth
                ? sourceHealth.transform.position
                : transform.position;

            var damageArgs = new HealthChangeEventArgs(amount, source, this, sourcePosition);
            RaiseOnHurt(damageArgs);

            if (IsAlive)
                return;

            LatestDeathSource = source;
            RaiseOnDie(damageArgs);
            enabled = false;
        }

        public override void Heal(float amount, IHealth source)
        {
            if (IsDead)
                return;

            CurrentHealth += amount;

            Vector3 sourcePosition = source is Health sourceHealth
                ? sourceHealth.transform.position
                : transform.position;

            var healArgs = new HealthChangeEventArgs(amount, source, this, sourcePosition);
            RaiseOnHeal(healArgs);
        }

        public override void Revive(IHealth reviver = null)
        {
            CurrentHealth = maxHealth;
            enabled = true;
            RaiseOnRevive(reviver ?? this);
        }

        [ContextMenu("Take 10 Damage")]
        public void Damage() => Damage(10f, this);

        private void OnValidate() => CurrentHealth = currentHealth;

        public override string ToString() => Name;
    }
}
