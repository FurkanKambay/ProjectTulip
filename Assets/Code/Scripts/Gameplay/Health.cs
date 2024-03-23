using System;
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

        public Health LastDamageSource { get; private set; }
        public Health LastDeathSource { get; private set; }

        public event Action<DamageEventArgs> OnHurt;
        public event Action<DamageEventArgs> OnDie;

        public void TakeDamage(float damage, Health source)
        {
            var self = (IHealth)this;
            if (self.IsDead) return;

            CurrentHealth -= damage;
            LastDamageSource = source;

            var damageArgs = new DamageEventArgs(damage, source, this);
            OnHurt?.Invoke(damageArgs);

            if (self.IsAlive) return;

            LastDeathSource = source;
            OnDie?.Invoke(damageArgs);
            enabled = false;
        }

        [ContextMenu("Take 10 Damage")]
        public void TakeDamage() => TakeDamage(10f, this);

        private void OnValidate() => CurrentHealth = currentHealth;
    }
}
