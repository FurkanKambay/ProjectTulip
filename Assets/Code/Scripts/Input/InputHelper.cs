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
            => OnSelectHotbar?.Invoke((int)context.ReadValue<float>());

        private void HandleInputPoint(InputAction.CallbackContext context)
            => MouseScreenPoint = context.ReadValue<Vector2>();

        private static void HandleGameStateChanged()
        {
            if (GameState.Current.IsPlayerInputEnabled)
            {
                Debug.Log($"[Input] + Player input enabled by {GameState.Current}.");
                Instance.Actions.Player.Hotbar.performed += Instance.HandleInputHotbar;
                Instance.Actions.Player.Point.performed += Instance.HandleInputPoint;
                Instance.Actions.Player.Enable();
                Instance.Actions.UI.Disable();
            }
            else
            {
                Debug.Log($"[Input] - Player input disabled by {GameState.Current}.");
                Instance.Actions.Player.Disable();
                Instance.Actions.UI.Enable();
            }
        }

        private void OnEnable()
        {
            Instance = this;
            GameState.OnGameStateChange += HandleGameStateChanged;
            Debug.Log("[Input] Enabled input helper.");
        }
    }
}
