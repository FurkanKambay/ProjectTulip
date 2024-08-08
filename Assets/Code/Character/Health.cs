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

        public override InventoryModification Damage(float amount, IHealth source, bool checkInvulnerable = true)
        {
            if (IsDead || amount < 0)
                return default;

            if (checkInvulnerable && IsInvulnerable)
                return default;

            CurrentHealth -= amount;
            LatestDamageSource = source;

            if (checkInvulnerable)
                remainingInvulnerability = invulnerabilityDuration;

            Vector3 sourcePosition = source is Health sourceHealth
                ? sourceHealth.transform.position
                : transform.position;

            var damageArgs = new HealthChangeEventArgs(amount, source, this, sourcePosition);
            RaiseOnHurt(damageArgs);

            if (IsAlive)
                return default;

            LatestDeathSource = source;
            RaiseOnDie(damageArgs);
            enabled = false;

            // TODO: fix whatever this is later
            Entity entity = Entity.Entity;

            if (!entity || !entity.Loot)
                return default;

            return InventoryModification.ToAdd(entity.Loot.Stack(entity.LootAmount));
        }

        public override void Heal(float amount, IHealth source)
        {
            if (IsDead || amount < 0)
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
