using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Gameplay
{
    [SelectionBase]
    public class Health : MonoBehaviour, IHealth
    {
        [SerializeField, Min(0)] float maxHealth = 100f;
        [SerializeField] float currentHealth = 100f;

        public float MaxHealth => maxHealth;

        public float CurrentHealth
        {
            get => currentHealth;
            private set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
        }

        public IHealth LatestDamageSource { get; private set; }
        public IHealth LatestDeathSource { get; private set; }

        public event IHealth.DamageEvent OnHurt;
        public event IHealth.DeathEvent OnDie;
        public event IHealth.ReviveEvent OnRevive;

        public void TakeDamage(float damage, IHealth source)
        {
            var self = (IHealth)this;
            if (self.IsDead) return;

            CurrentHealth -= damage;
            LatestDamageSource = source;

            Vector3 sourcePosition = source is Health sourceHealth ? sourceHealth.transform.position : transform.position;
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
