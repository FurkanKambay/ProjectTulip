using System;
using Tulip.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Input
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

        private void HandleGameStateChanged()
        {
            switch (Bootstrapper.GameState)
            {
                case GameState.InGame:
                    Actions.Player.Hotbar.performed += HandleInputHotbar;
                    Actions.Player.Point.performed += HandleInputPoint;
                    Actions.Player.Enable();
                    Actions.UI.Disable();
                    break;
                case GameState.InMainMenu:
                case GameState.Paused:
                default:
                    Actions.Player.Disable();
                    Actions.UI.Enable();
                    break;
            }
        }

        private void OnEnable() => Bootstrapper.OnGameStateChange += HandleGameStateChanged;
        private void OnDisable() => Bootstrapper.OnGameStateChange -= HandleGameStateChanged;
        private void OnApplicationQuit() => Bootstrapper.OnGameStateChange -= HandleGameStateChanged;
    }
}
