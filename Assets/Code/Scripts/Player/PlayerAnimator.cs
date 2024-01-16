using Game.CharacterController;
using Game.Data.Interfaces;
using UnityEngine;

namespace Game.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        private static readonly int animSpeed = Animator.StringToHash("speed");
        private static readonly int animJump = Animator.StringToHash("jump");

        private Animator animator;
        private IMovement movement;
        private PlayerJump playerJump;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            movement = GetComponentInParent<IMovement>();
            playerJump = GetComponentInParent<PlayerJump>();
        }

        private void Update()
        {
            animator.SetFloat(animSpeed, Mathf.Abs(movement.Velocity.x));
            animator.SetBool(animJump, playerJump.CurrentlyJumping);
        }
    }
}
