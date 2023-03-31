using UnityEngine;

namespace Game
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

        [ContextMenu("Take Damage")]
        public void TakeDamage() => TakeDamage(10f);

        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
                Die();

            print($"Ouch! {currentHealth}/{maxHealth}.");
        }

        public void Die() => Destroy(gameObject);
    }
}
