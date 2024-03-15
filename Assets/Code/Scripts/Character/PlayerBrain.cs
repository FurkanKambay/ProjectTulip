using Tulip.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Character
{
    public class PlayerBrain : CharacterBrain
    {
        private void RaisePlayerMoveLateral(InputAction.CallbackContext context) =>
            RaiseOnMoveLateral(context.ReadValue<float>());

        private void RaisePlayerJump(InputAction.CallbackContext _) => RaiseOnJump();
        private void RaisePlayerJumpReleased(InputAction.CallbackContext _) => RaiseOnJumpReleased();

        private void OnEnable()
        {
            InputHelper.Instance.Actions.Player.MoveX.performed += RaisePlayerMoveLateral;
            InputHelper.Instance.Actions.Player.Jump.performed += RaisePlayerJump;
            InputHelper.Instance.Actions.Player.Jump.canceled += RaisePlayerJumpReleased;
        }

        private void OnDisable()
        {
            InputHelper.Instance.Actions.Player.MoveX.performed -= RaisePlayerMoveLateral;
            InputHelper.Instance.Actions.Player.Jump.performed -= RaisePlayerJump;
            InputHelper.Instance.Actions.Player.Jump.canceled -= RaisePlayerJumpReleased;
        }
    }
}
