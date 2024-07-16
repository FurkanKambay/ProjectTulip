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

        private async void HandleHurt(DamageEventArgs damage)
        {
            if (hurtVisualDuration <= 0)
                return;

            materialBlock.SetInt(shaderReplaceColor, 1);
            sprite.SetPropertyBlock(materialBlock);

            await Awaitable.WaitForSecondsAsync(hurtVisualDuration);

            materialBlock.SetInt(shaderReplaceColor, 0);
            sprite.SetPropertyBlock(materialBlock);
        }

        private void HandleRevived(IHealth source)
        {
            sprite.sortingOrder = 0;

            StopAllCoroutines();
            StartCoroutine(DissolveSprite(dissolveDuration / 2f, 1, 0));
        }

        private void HandleDied(DamageEventArgs damage)
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

                materialBlock.SetFloat(shaderDissolveAmount, amount);
                sprite.SetPropertyBlock(materialBlock);

                yield return null;
            }

            if (destroyAfterDeath)
                Destroy(health.transform.parent.gameObject);
        }
    }
}
