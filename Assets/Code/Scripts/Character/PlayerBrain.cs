using System;
using Tulip.Data;
using Tulip.Data.Gameplay;
using Tulip.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Character
{
    public class PlayerBrain : MonoBehaviour, IDasherBrain, IJumperBrain, IWielderBrain
    {
        public event Action OnJump;
        public event Action OnJumpReleased;

        public float HorizontalMovement { get; private set; }
        public bool WantsToDash { get; private set; }

        public bool WantsToUse { get; private set; }
        public Vector3 AimPosition { get; private set; }

        private Camera mainCamera;
        private IHealth health;

        private void Awake()
        {
            mainCamera = Camera.main;
            health = GetComponent<IHealth>();
        }

        private void Update()
        {
            if (health.IsDead)
            {
                OnJumpReleased?.Invoke();
                return;
            }

            AimPosition = mainCamera.ScreenToWorldPoint(InputHelper.Instance.MouseScreenPoint);
            HorizontalMovement = InputHelper.Instance.Actions.Player.MoveX.ReadValue<float>();
            WantsToUse = InputHelper.Instance.Actions.Player.Use.inProgress;
            WantsToDash = InputHelper.Instance.Actions.Player.Dash.inProgress;
        }

        private void OnEnable()
        {
            InputHelper.Instance.Actions.Player.Jump.performed += RaisePlayerJump;
            InputHelper.Instance.Actions.Player.Jump.canceled += RaisePlayerJumpReleased;
        }

        private void OnDisable()
        {
            InputHelper.Instance.Actions.Player.Jump.performed -= RaisePlayerJump;
            InputHelper.Instance.Actions.Player.Jump.canceled -= RaisePlayerJumpReleased;
        }

        private void RaisePlayerJump(InputAction.CallbackContext _)
        {
            if (health.IsDead) return;
            OnJump?.Invoke();
        }

        private void RaisePlayerJumpReleased(InputAction.CallbackContext _)
        {
            if (health.IsDead) return;
            OnJumpReleased?.Invoke();
        }
    }
}
