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

        public event Action<float> DamageTaken;
        public event Action Died;

        [ContextMenu("Take Damage")]
        public void TakeDamage() => TakeDamage(10f);

        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            DamageTaken?.Invoke(damage);

            if (CurrentHealth <= 0)
                Died?.Invoke();
        }
    }
}
