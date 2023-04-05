using System;
using Game.Gameplay;
using UnityEngine;

namespace Game.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private float changeSpeed = 10f;

        private Health health;
        private SpriteRenderer healthBarSprite;
        private float targetValue;

        private static readonly int healthShaderValue = Shader.PropertyToID("_Value");

        private void Awake()
        {
            health = GetComponentInParent<Health>();
            healthBarSprite = GetComponent<SpriteRenderer>();
            healthBarSprite.enabled = false;
            targetValue = health.maxHealth;
        }

        private void Update()
        {
            bool isFull = Math.Abs(health.CurrentHealth - health.maxHealth) < .01f;
            healthBarSprite.enabled = health.showHealthBar && !isFull;

            if (!health.showHealthBar) return;

            float value = health.CurrentHealth / health.maxHealth;
            targetValue = Mathf.Lerp(targetValue, value, changeSpeed * Time.deltaTime);
            healthBarSprite.material.SetFloat(healthShaderValue, targetValue);
        }
    }
}
