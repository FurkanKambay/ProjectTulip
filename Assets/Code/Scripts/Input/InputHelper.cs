using System;
using Game.Helpers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public class InputHelper : PersistentSingleton<InputHelper>
    {
        public static InputActions Actions => Instance.actions ??= new InputActions();

        public event Action<int> OnSelectHotbar;
        public Vector2 MouseScreenPoint { get; private set; }

        private InputActions actions;

        private void HandleInputHotbar(InputAction.CallbackContext context)
            => OnSelectHotbar?.Invoke(Convert.ToInt32(context.control.name) - 1);

        private void HandleInputPoint(InputAction.CallbackContext context)
            => MouseScreenPoint = context.ReadValue<Vector2>();

        private void OnEnable()
        {
            Actions.Enable();
            Actions.Player.Hotbar.performed += HandleInputHotbar;
            Actions.Player.Point.performed += HandleInputPoint;
        }

        private void OnDisable()
        {
            Actions.Player.Hotbar.performed -= HandleInputHotbar;
            Actions.Player.Point.performed -= HandleInputPoint;
            Actions.Disable();
        }
    }
}
