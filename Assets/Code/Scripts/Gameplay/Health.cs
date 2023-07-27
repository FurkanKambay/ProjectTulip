using System;
using Game.Data.Interfaces;
using UnityEngine;

namespace Game.Gameplay
{
    public class Health : MonoBehaviour, IHealth
    {
        [SerializeField, Min(0)] float maxHealth = 100f;
        [SerializeField] float currentHealth = 100f;

        public float MaxHealth => maxHealth;
        public float CurrentHealth
        {
            get => currentHealth;
            set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
        }

        public event Action<DamageEventArgs> DamageTaken;
        public event Action<DamageEventArgs> Died;

        public void TakeDamage(float damage, Health source)
        {
            CurrentHealth -= damage;

            var eventArgs = new DamageEventArgs(damage, source, this);
            DamageTaken?.Invoke(eventArgs);

            if (CurrentHealth > 0) return;
            Died?.Invoke(eventArgs);
            enabled = false;
        }

        [ContextMenu("Take Damage")]
        public void TakeDamage() => TakeDamage(10f, this);

        private void OnValidate() => CurrentHealth = currentHealth;
    }
}
