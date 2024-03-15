using System;
using Tulip.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Input
{
    public class InputHelper : ScriptableObject
    {
        private static InputHelper instance;

        public event Action<int> OnSelectHotbar;

        public InputActions Actions => actions ??= new InputActions();
        private InputActions actions;

        public Vector2 MouseScreenPoint { get; private set; }

        private void HandleInputHotbar(InputAction.CallbackContext context)
            => OnSelectHotbar?.Invoke(Convert.ToInt32(context.control.name) - 1);

        private void HandleInputPoint(InputAction.CallbackContext context)
            => MouseScreenPoint = context.ReadValue<Vector2>();

        private static void HandleGameStateChanged()
        {
            if (GameState.Current == GameState.Playing)
            {
                instance.Actions.Player.Hotbar.performed += instance.HandleInputHotbar;
                instance.Actions.Player.Point.performed += instance.HandleInputPoint;
                instance.Actions.Player.Enable();
                instance.Actions.UI.Disable();
            }
            else
            {
                instance.Actions.Player.Disable();
                instance.Actions.UI.Enable();
            }
        }

        private void OnEnable()
        {
            instance = this;
            GameState.OnGameStateChange += HandleGameStateChanged;
            Debug.Log("Enabled input helper.");
        }
    }
}
