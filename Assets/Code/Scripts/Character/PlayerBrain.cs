using Tulip.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Character
{
    public class PlayerBrain : CharacterBrain
    {
        [SerializeField] private InputHelper inputHelper;

        private void RaisePlayerMoveLateral(InputAction.CallbackContext context) =>
            RaiseOnMoveLateral(context.ReadValue<float>());

        private void RaisePlayerJump(InputAction.CallbackContext _) => RaiseOnJump();
        private void RaisePlayerJumpReleased(InputAction.CallbackContext _) => RaiseOnJumpReleased();

        private void OnEnable()
        {
            inputHelper.Actions.Player.MoveX.performed += RaisePlayerMoveLateral;
            inputHelper.Actions.Player.Jump.performed += RaisePlayerJump;
            inputHelper.Actions.Player.Jump.canceled += RaisePlayerJumpReleased;
        }

        private void OnDisable()
        {
            inputHelper.Actions.Player.MoveX.performed -= RaisePlayerMoveLateral;
            inputHelper.Actions.Player.Jump.performed -= RaisePlayerJump;
            inputHelper.Actions.Player.Jump.canceled -= RaisePlayerJumpReleased;
        }
    }
}
