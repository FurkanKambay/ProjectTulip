using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Data
{
    public abstract class HealthBase : MonoBehaviour, IHealth
    {
        public virtual event IHealth.DamageEvent OnHurt;
        public virtual event IHealth.DeathEvent OnDie;
        public virtual event IHealth.ReviveEvent OnRevive;

        [Header("Config")]
        [SerializeField, Min(0)] protected float maxHealth = 100f;
        [SerializeField, Min(0)] protected float currentHealth = 100f;
        [SerializeField, Min(0)] protected float invulnerabilityDuration;

        [Header("Entity")]
        [field: SerializeField] public virtual string Name { get; protected set; }

        public virtual float CurrentHealth
        {
            get => currentHealth;
            protected set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
        }

        public virtual float MaxHealth => maxHealth;
        public virtual float Ratio => CurrentHealth / MaxHealth;
        public virtual bool IsAlive => CurrentHealth > 0;
        public virtual bool IsDead => CurrentHealth <= 0;
        public virtual bool IsFull => CurrentHealth >= MaxHealth;
        public virtual bool IsHurt => CurrentHealth < MaxHealth && !IsDead;
        public virtual bool IsInvulnerable => remainingInvulnerability > 0;

        public virtual IHealth LatestDamageSource { get; protected set; }
        public virtual IHealth LatestDeathSource { get; protected set; }

        /// Remaining seconds of invulnerability
        protected float remainingInvulnerability;

        public abstract void TakeDamage(float damage, IHealth source);
        public abstract void Revive(IHealth source = null);

        protected void RaiseOnHurt(DamageEventArgs damage) => OnHurt?.Invoke(damage);
        protected void RaiseOnDie(DamageEventArgs death) => OnDie?.Invoke(death);
        protected void RaiseOnRevive(IHealth source) => OnRevive?.Invoke(source);
    }
}
