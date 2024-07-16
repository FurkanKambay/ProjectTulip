using System.Collections;
using SaintsField;
using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Gameplay
{
    public class CharacterAnimator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] HealthBase health;
        [SerializeField, Required] Animator animator;
        [SerializeField, Required] SpriteRenderer sprite;
        [SerializeField, Required] SaintsInterface<Object, ICharacterMovement> movement;
        [SerializeField, Required] SaintsInterface<Object, ICharacterJump> jumper;

        [Header("Config")]

        [OverlayRichLabel("<color=gray>sec")]
        [SerializeField, Min(0)] float hurtVisualDuration;

        [OverlayRichLabel("<color=gray>sec")]
        [SerializeField, Min(0)] float dissolveDuration = 1f;

        [SerializeField] bool destroyAfterDeath;

        private MaterialPropertyBlock materialBlock;

        private static readonly int animSpeed = Animator.StringToHash("speed");
        private static readonly int animJumping = Animator.StringToHash("jumping");
        private static readonly int animHurt = Animator.StringToHash("hurt");
        private static readonly int animDead = Animator.StringToHash("dead");
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

        private void Update() =>
            animator.SetBool(animJumping, jumper.I.IsJumping);

        private void FixedUpdate() =>
            animator.SetFloat(animSpeed, Mathf.Abs(movement.I.Velocity.x));

        private async void HandleHurt(DamageEventArgs damage)
        {
            animator.SetTrigger(animHurt);
            await GetHurt();
        }

        private async Awaitable GetHurt()
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
            animator.SetBool(animDead, false);

            StopAllCoroutines();
            StartCoroutine(DissolveSprite(dissolveDuration / 2f, 1, 0));
        }

        private void HandleDied(DamageEventArgs damage)
        {
            sprite.sortingOrder = -1;
            animator.SetBool(animDead, true);

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
