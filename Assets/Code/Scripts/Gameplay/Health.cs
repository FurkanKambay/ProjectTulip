using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Gameplay
{
    [SelectionBase]
    public class Health : MonoBehaviour, IHealth
    {
        public event IHealth.DamageEvent OnHurt;
        public event IHealth.DeathEvent OnDie;
        public event IHealth.ReviveEvent OnRevive;

        [SerializeField, Min(0)] float maxHealth = 100f;
        [SerializeField, Min(0)] float currentHealth = 100f;
        [SerializeField, Min(0)] float invulnerabilityDuration;

        public float CurrentHealth
        {
            get => currentHealth;
            private set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
        }

        public float MaxHealth => maxHealth;
        public bool IsInvulnerable => remainingSecondsOfInvulnerability > 0;

        public IHealth LatestDamageSource { get; private set; }
        public IHealth LatestDeathSource { get; private set; }

        private float remainingSecondsOfInvulnerability;

        private void Update() =>
            remainingSecondsOfInvulnerability = Mathf.Max(0, remainingSecondsOfInvulnerability - Time.deltaTime);

        public void TakeDamage(float damage, IHealth source)
        {
            if (IsInvulnerable) return;

            var self = (IHealth)this;
            if (self.IsDead) return;

            CurrentHealth -= damage;
            LatestDamageSource = source;
            remainingSecondsOfInvulnerability = invulnerabilityDuration;

            Vector3 sourcePosition = source is Health sourceHealth
                ? sourceHealth.transform.position
                : transform.position;

            var damageArgs = new DamageEventArgs(damage, source, this, sourcePosition);
            OnHurt?.Invoke(damageArgs);

            if (self.IsAlive) return;

            LatestDeathSource = source;
            OnDie?.Invoke(damageArgs);
            enabled = false;
        }

        public void Revive(IHealth source = null)
        {
            CurrentHealth = maxHealth;
            enabled = true;
            OnRevive?.Invoke(source ?? this);
        }

        [ContextMenu("Take 10 Damage")]
        public void TakeDamage() => TakeDamage(10f, this);

        private void OnValidate() => CurrentHealth = currentHealth;
    }
}
