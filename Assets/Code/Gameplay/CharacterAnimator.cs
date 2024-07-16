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

        private static readonly int animSpeed = Animator.StringToHash("speed");
        private static readonly int animJumping = Animator.StringToHash("jumping");
        private static readonly int animHurt = Animator.StringToHash("hurt");
        private static readonly int animDead = Animator.StringToHash("dead");

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

        private void Update() => animator.SetBool(animJumping, jumper.I.IsJumping);
        private void FixedUpdate() => animator.SetFloat(animSpeed, Mathf.Abs(movement.I.Velocity.x));

        private void HandleHurt(DamageEventArgs damage) => animator.SetTrigger(animHurt);
        private void HandleDied(DamageEventArgs damage) => animator.SetBool(animDead, true);
        private void HandleRevived(IHealth source) => animator.SetBool(animDead, false);
    }
}
