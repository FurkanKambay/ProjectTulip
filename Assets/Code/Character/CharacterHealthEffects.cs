using System.Collections;
using SaintsField;
using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Character
{
    public class CharacterHealthEffects : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] HealthBase health;
        [SerializeField, Required] SpriteRenderer sprite;

        [Header("Config")]
        [SerializeField] bool destroyAfterDeath;

        [OverlayRichLabel("<color=gray>sec")]
        [SerializeField, Min(0)] float hurtVisualDuration = 0.1f;

        [OverlayRichLabel("<color=gray>sec")]
        [SerializeField, Min(0)] float dissolveDuration = 1f;

        private MaterialPropertyBlock materialBlock;

        private static readonly int shaderReplaceColor = Shader.PropertyToID("_ReplaceColor");
        private static readonly int shaderDissolveAmount = Shader.PropertyToID("_DissolveAmount");
        private static readonly int shaderOutlineThickness = Shader.PropertyToID("_Outline_Thickness");

        private void OnEnable()
        {
            materialBlock = new MaterialPropertyBlock();

            health.OnHurt += HandleHurt;
            health.OnDie += HandleDied;
            health.OnRevive += HandleRevived;
        }

        private void OnDisable()
        {
            health.OnHurt -= HandleHurt;
            health.OnDie -= HandleDied;
            health.OnRevive -= HandleRevived;
        }

        private async void HandleHurt(HealthChangeEventArgs damage)
        {
            if (hurtVisualDuration > 0)
            {
                ApplyShaderProperty(shaderReplaceColor, 1);
                await Awaitable.WaitForSecondsAsync(hurtVisualDuration);
                ApplyShaderProperty(shaderReplaceColor, 0);
            }

            ApplyShaderProperty(shaderOutlineThickness, 1);
            await Awaitable.WaitForSecondsAsync(health.InvulnerabilityDuration - hurtVisualDuration);
            ApplyShaderProperty(shaderOutlineThickness, 0);
        }

        private void HandleRevived(IHealth reviver)
        {
            sprite.sortingOrder = 0;

            StopAllCoroutines();
            StartCoroutine(DissolveSprite(dissolveDuration / 2f, 1, 0));
        }

        private void HandleDied(HealthChangeEventArgs damage)
        {
            sprite.sortingOrder = -1;

            StopAllCoroutines();
            StartCoroutine(DissolveSprite(dissolveDuration));
        }

        private IEnumerator DissolveSprite(float duration, float startAmount = 0, float endAmount = 1)
        {
            float amount = startAmount;

            while (!Mathf.Approximately(amount, endAmount))
            {
                amount = Mathf.MoveTowards(amount, endAmount, Time.deltaTime / duration);

                ApplyShaderProperty(shaderDissolveAmount, amount);
                yield return null;
            }

            if (destroyAfterDeath)
                Destroy(health.transform.parent.gameObject);
        }

        private void ApplyShaderProperty(int shaderProperty, float value)
        {
            materialBlock.SetFloat(shaderProperty, value);
            sprite.SetPropertyBlock(materialBlock);
        }
    }
}
