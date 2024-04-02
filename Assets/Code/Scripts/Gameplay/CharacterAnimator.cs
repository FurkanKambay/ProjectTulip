using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Gameplay
{
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] Health health;
        [SerializeField] Animator animator;
        [SerializeField] bool destroyAfterDeath;

        private ICharacterMovement movement;
        private ICharacterJump jumper;

        private static readonly int animSpeed = Animator.StringToHash("speed");
        private static readonly int animJumping = Animator.StringToHash("jumping");
        private static readonly int animHurt = Animator.StringToHash("hurt");
        private static readonly int animDead = Animator.StringToHash("dead");

        private void Update() => animator.SetBool(animJumping, jumper.IsJumping);
        private void FixedUpdate() => animator.SetFloat(animSpeed, Mathf.Abs(movement.Velocity.x));

        private void HandleHurt(DamageEventArgs damage) => animator.SetTrigger(animHurt);
        private void HandleRevived(IHealth source) => animator.SetBool(animDead, false);

        private void HandleDied(DamageEventArgs damage)
        {
            animator.SetBool(animDead, true);

            if (destroyAfterDeath)
                DestroyAfterAnimation(.5f);
        }

        private void DestroyAfterAnimation(float extraDelay = 0f)
        {
            float length = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(health.gameObject, length + extraDelay);
        }

        private void Awake()
        {
            animator ??= GetComponent<Animator>();
            health ??= GetComponentInParent<Health>();
            movement = health.GetComponent<ICharacterMovement>();
            jumper = health.GetComponent<ICharacterJump>();
        }

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
    }
}
