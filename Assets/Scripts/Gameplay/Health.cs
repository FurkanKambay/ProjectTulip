using System;
using UnityEngine;

namespace Game.Gameplay
{
    public class Health : MonoBehaviour
    {
        public float maxHealth = 100f;
        [SerializeField] private float currentHealth = 100f;

        public float CurrentHealth
        {
            get => currentHealth;
            set => currentHealth = Mathf.Min(value, maxHealth);
        }

        public event Action<DamageEventArgs> DamageTaken;
        public event Action<DamageEventArgs> Died;

        [ContextMenu("Take Damage")]
        public void TakeDamage() => TakeDamage(10f, this);

        public void TakeDamage(float damage, Health source)
        {
            CurrentHealth -= damage;

            var eventArgs = new DamageEventArgs(damage, source, this);
            DamageTaken?.Invoke(eventArgs);

            if (CurrentHealth > 0) return;
            Died?.Invoke(eventArgs);
            enabled = false;
        }
    }
}
