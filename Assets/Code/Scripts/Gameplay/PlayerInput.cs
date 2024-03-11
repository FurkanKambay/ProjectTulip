using Tulip.Data;
using Tulip.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Gameplay
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private InputHelper inputHelper;

        private ICharacterMovement movement;

        private void Awake() => movement = GetComponent<ICharacterMovement>();

        private void OnMoveX(InputAction.CallbackContext context)
            => movement.Input = new Vector2(context.ReadValue<float>(), 0f);

        private void OnEnable() => inputHelper.Actions.Player.MoveX.performed += OnMoveX;
        private void OnDisable() => inputHelper.Actions.Player.MoveX.performed -= OnMoveX;
    }
}
