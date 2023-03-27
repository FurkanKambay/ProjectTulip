using UnityEngine;

namespace Game.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        private static readonly int animSpeed = Animator.StringToHash("speed");
        private static readonly int animJump = Animator.StringToHash("jump");

        private Animator animator;

        private void Awake() => animator = GetComponent<Animator>();

        private void Update()
        {
            float moveX = Input.Actions.Player.MoveX.ReadValue<float>();
            animator.SetFloat(animSpeed, Mathf.Abs(moveX));

            bool jump = Input.Actions.Player.Jump.triggered;
            animator.SetBool(animJump, jump);
        }
    }
}
