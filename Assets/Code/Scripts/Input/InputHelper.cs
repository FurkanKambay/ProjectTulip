using System;
using Tulip.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Input
{
    public class InputHelper : ScriptableObject
    {
        public static InputHelper Instance { get; private set; }

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
                Instance.Actions.Player.Hotbar.performed += Instance.HandleInputHotbar;
                Instance.Actions.Player.Point.performed += Instance.HandleInputPoint;
                Instance.Actions.Player.Enable();
                Instance.Actions.UI.Disable();
            }
            else
            {
                Instance.Actions.Player.Disable();
                Instance.Actions.UI.Enable();
            }
        }

        private void OnEnable()
        {
            Instance = this;
            GameState.OnGameStateChange += HandleGameStateChanged;
            Debug.Log("Enabled input helper.");
        }
    }
}
