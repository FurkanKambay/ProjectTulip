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
        public Vector2 MouseWorldPoint => mainCamera.ScreenToWorldPoint(screenPoint);

        private InputActions actions;
        private Camera mainCamera;
        private Vector2 screenPoint;

        private void HandleInputHotbar(InputAction.CallbackContext context)
            => OnSelectHotbar?.Invoke(Convert.ToInt32(context.control.name) - 1);

        private void HandleInputPoint(InputAction.CallbackContext context)
            => screenPoint = context.ReadValue<Vector2>();

        protected override void Awake()
        {
            base.Awake();

            mainCamera = Camera.main;
            Actions.Player.Hotbar.performed += HandleInputHotbar;
            Actions.Player.Point.performed += HandleInputPoint;
        }

        private void OnEnable() => Actions.Enable();
        private void OnDisable() => Actions.Disable();
    }
}
