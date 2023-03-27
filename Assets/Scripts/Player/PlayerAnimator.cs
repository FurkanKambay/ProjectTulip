using System;
using Game.Data.Items;
using UnityEngine;

namespace Game.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        private static readonly int animSpeed = Animator.StringToHash("speed");
        private static readonly int animJump = Animator.StringToHash("jump");
        private static readonly int animAttack = Animator.StringToHash("attack");
        private static readonly int animAttackSpeed = Animator.StringToHash("attack speed");

        private Animator animator;
        private Inventory inventory;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            inventory = GetComponentInParent<Inventory>();
        }

        private void Start()
        {
            inventory.HotbarSelectionChanged += _ => OnHotbarSelection(inventory.HotbarSelected);
            OnHotbarSelection(inventory.HotbarSelected);
        }

        private void OnHotbarSelection(IUsable item)
            => animator.SetFloat(animAttackSpeed, 1 / item.UseTime);

        private void Update()
        {
            animator.SetFloat(animSpeed, Mathf.Abs(Input.Actions.Player.MoveX.ReadValue<float>()));
            animator.SetBool(animJump, Input.Actions.Player.Jump.inProgress);
            animator.SetBool(animAttack, Input.Actions.Player.Fire.inProgress);
        }
    }
}
