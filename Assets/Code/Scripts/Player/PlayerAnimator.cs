using Tulip.Data;
using UnityEngine;

namespace Tulip.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator animator;
        private ICharacterMovement movement;
        private ICharacterJump playerJump;

        private static readonly int animSpeed = Animator.StringToHash("speed");
        private static readonly int animJump = Animator.StringToHash("jump");

        private void Awake()
        {
            animator = GetComponent<Animator>();
            movement = GetComponentInParent<ICharacterMovement>();
            playerJump = GetComponentInParent<ICharacterJump>();
        }

        private void Update()
        {
            animator.SetFloat(animSpeed, Mathf.Abs(movement.Velocity.x));
            animator.SetBool(animJump, playerJump.IsJumping);
        }
    }
}
