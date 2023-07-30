using Game.CharacterController;
using Game.Data.Interfaces;
using Game.Input;
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
        private WorldModifier worldModifier;
        private IMovement movement;
        private PlayerJump playerJump;

        private InputActions.PlayerActions playerActions;

        private void Awake()
        {
            playerActions = InputHelper.Actions.Player;

            animator = GetComponent<Animator>();
            inventory = GetComponentInParent<Inventory>();
            worldModifier = GetComponentInParent<WorldModifier>();
            movement = GetComponentInParent<IMovement>();
            playerJump = GetComponentInParent<PlayerJump>();
        }

        private void Start()
        {
            inventory.HotbarSelectionChanged += OnHotbarSelection;
            OnHotbarSelection(inventory.HotbarSelectedIndex);
        }

        private void OnHotbarSelection(int index)
        {
            if (inventory.HotbarSelected?.Item is not IUsable item) return;
            animator.SetFloat(animAttackSpeed, 1f / item.Cooldown);
        }

        private void Update()
        {
            bool canPlayAttack = inventory.HotbarSelected?.Item switch
            {
                ITool => worldModifier && worldModifier.FocusedCell.HasValue && playerActions.Use.inProgress,
                Weapon => playerActions.Use.inProgress,
                _ => false
            };

            animator.SetBool(animAttack, canPlayAttack);
            animator.SetFloat(animSpeed, Mathf.Abs(movement.Velocity.x));
            animator.SetBool(animJump, playerJump.CurrentlyJumping);
        }
    }
}