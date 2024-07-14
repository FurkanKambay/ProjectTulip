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

        [SerializeField] bool destroyAfterDeath;

        private static readonly int animSpeed = Animator.StringToHash("speed");
        private static readonly int animJumping = Animator.StringToHash("jumping");
        private static readonly int animHurt = Animator.StringToHash("hurt");
        private static readonly int animDead = Animator.StringToHash("dead");
        private static readonly int shaderReplaceColor = Shader.PropertyToID("_ReplaceColor");

        private void OnEnable()
        {
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

            var block = new MaterialPropertyBlock();
            block.SetInt(shaderReplaceColor, 1);

            sprite.SetPropertyBlock(block);
            await Awaitable.WaitForSecondsAsync(hurtVisualDuration);

            block.SetInt(shaderReplaceColor, 0);
            sprite.SetPropertyBlock(block);
        }

        private void HandleRevived(IHealth source) =>
            animator.SetBool(animDead, false);

        private void HandleDied(DamageEventArgs damage)
        {
            animator.SetBool(animDead, true);

            if (destroyAfterDeath)
                DestroyAfterAnimation(.5f);
        }

        private void DestroyAfterAnimation(float extraDelay = 0f)
        {
            float length = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(health.transform.parent.gameObject, length + extraDelay);
        }
    }
}
