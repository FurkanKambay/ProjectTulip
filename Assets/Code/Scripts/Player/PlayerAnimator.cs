using Tulip.CharacterController;
using Tulip.Data.Interfaces;
using UnityEngine;

namespace Tulip.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator animator;
        private IMovement movement;
        private PlayerJump playerJump;

        private static readonly int animSpeed = Animator.StringToHash("speed");
        private static readonly int animJump = Animator.StringToHash("jump");

        private void Awake()
        {
            animator = GetComponent<Animator>();
            movement = GetComponentInParent<IMovement>();
            playerJump = GetComponentInParent<PlayerJump>();
        }

        private void Update()
        {
            animator.SetFloat(animSpeed, Mathf.Abs(movement.Velocity.x));
            animator.SetBool(animJump, playerJump.IsJumping);
        }
    }
}
