using Game.CharacterController;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Gameplay
{
    public class PlayerInput : MonoBehaviour
    {
        private CharacterMovement movement;

        private void Awake() => movement = GetComponent<CharacterMovement>();
        private void Start() => InputHelper.Actions.Player.MoveX.performed += OnMoveX;

        private void OnMoveX(InputAction.CallbackContext context)
            => movement.Input = new Vector2(context.ReadValue<float>(), 0f);
    }
}
