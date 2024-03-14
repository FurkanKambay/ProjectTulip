using System;
using Tulip.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Input
{
    public class InputHelper : ScriptableObject
    {
        public event Action<int> OnSelectHotbar;

        public InputActions Actions => actions ??= new InputActions();
        private InputActions actions;

        public Vector2 MouseScreenPoint { get; private set; }

        private void HandleInputHotbar(InputAction.CallbackContext context)
            => OnSelectHotbar?.Invoke(Convert.ToInt32(context.control.name) - 1);

        private void HandleInputPoint(InputAction.CallbackContext context)
            => MouseScreenPoint = context.ReadValue<Vector2>();

        private void HandleGameStateChanged()
        {
            if (Bootstrapper.GameState == GameState.Playing)
            {
                Actions.Player.Hotbar.performed += HandleInputHotbar;
                Actions.Player.Point.performed += HandleInputPoint;
                Actions.Player.Enable();
                Actions.UI.Disable();
            }
            else
            {
                Actions.Player.Disable();
                Actions.UI.Enable();
            }
        }

        private void OnEnable() => Bootstrapper.OnGameStateChange += HandleGameStateChanged;
        private void OnDisable() => Bootstrapper.OnGameStateChange -= HandleGameStateChanged;
        private void OnApplicationQuit() => Bootstrapper.OnGameStateChange -= HandleGameStateChanged;
    }
}
