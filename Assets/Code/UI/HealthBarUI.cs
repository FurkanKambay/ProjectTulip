using SaintsField;
using Tulip.Data;
using UnityEngine;

namespace Tulip.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] HealthBase health;
        [SerializeField, Required] SpriteRenderer healthBarSprite;

        [Header("Config")]
        [SerializeField] float changeSpeed = 10f;
        [SerializeField] bool showBar = true;

        private float targetValue;

        private static readonly int healthShaderValue = Shader.PropertyToID("_Value");

        private void Awake()
        {
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
