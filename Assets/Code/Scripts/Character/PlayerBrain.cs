using Tulip.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Character
{
    public class PlayerBrain : WielderBrain
    {
        private void RaisePlayerMoveLateral(InputAction.CallbackContext context) =>
            RaiseOnMoveLateral(context.ReadValue<float>());

        private void RaisePlayerJump(InputAction.CallbackContext _) => RaiseOnJump();
        private void RaisePlayerJumpReleased(InputAction.CallbackContext _) => RaiseOnJumpReleased();

        public override Vector3 FocusPosition { get; protected set; }

        private Camera mainCamera;

        private void Awake() => mainCamera = Camera.main;

        private void Update()
        {
            FocusPosition = mainCamera.ScreenToWorldPoint(InputHelper.Instance.MouseScreenPoint);
            IsUseInProgress = InputHelper.Instance.Actions.Player.Use.inProgress;
        }

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
