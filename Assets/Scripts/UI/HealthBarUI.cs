using Game.Data.Interfaces;
using UnityEngine;

namespace Game.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] float changeSpeed = 10f;
        [SerializeField] bool showBar = true;

        private IHealth health;
        private SpriteRenderer healthBarSprite;
        private float targetValue;

        private static readonly int healthShaderValue = Shader.PropertyToID("_Value");

        private void Awake()
        {
            health = GetComponentInParent<IHealth>();
            healthBarSprite = GetComponent<SpriteRenderer>();
            healthBarSprite.enabled = false;
            targetValue = health.MaxHealth;
        }

        private void Update()
        {
            healthBarSprite.enabled = showBar && health.IsHurt;
            if (!showBar) return;

            targetValue = Mathf.Lerp(targetValue, health.Ratio, changeSpeed * Time.deltaTime);
            healthBarSprite.material.SetFloat(healthShaderValue, targetValue);
        }
    }
}
