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

        private InputActions.PlayerActions playerActions;

        private void Awake()
        {
            playerActions = Input.Actions.Player;

            animator = GetComponent<Animator>();
            inventory = GetComponentInParent<Inventory>();
            worldModifier = GetComponentInParent<WorldModifier>();
        }

        private void Start()
        {
            inventory.HotbarSelectionChanged += OnHotbarSelection;
            OnHotbarSelection(inventory.HotbarSelectedIndex);
        }

        private void OnHotbarSelection(int index)
        {
            float? useTime = inventory.HotbarSelected?.UseTime;
            if (useTime.HasValue)
                animator.SetFloat(animAttackSpeed, 1f / useTime.Value);
        }

        private void Update()
        {
            bool canPlayAttack = worldModifier.FocusedCell.HasValue && playerActions.Fire.inProgress;
            animator.SetBool(animAttack, canPlayAttack);

            animator.SetFloat(animSpeed, Mathf.Abs(playerActions.MoveX.ReadValue<float>()));
            animator.SetBool(animJump, playerActions.Jump.inProgress);
        }
    }
}
