using System;
using Game.Helpers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class Input : PersistentSingleton<Input>
    {
        public static InputActions Actions => Instance.actions ??= new InputActions();

        public event Action<int> HotbarSelected;
        public Vector2 MouseWorldPoint => mainCamera.ScreenToWorldPoint(screenPoint);

        private InputActions actions;
        private Camera mainCamera;
        private Vector2 screenPoint;

        private void OnHotbar(InputAction.CallbackContext context)
            => HotbarSelected?.Invoke(Convert.ToInt32(context.control.name) - 1);

        private void OnPoint(InputAction.CallbackContext context)
            => screenPoint = context.ReadValue<Vector2>();

        protected override void Awake()
        {
            base.Awake();

            mainCamera = Camera.main;
            Actions.Player.Hotbar.performed += OnHotbar;
            Actions.Player.Point.performed += OnPoint;
        }

        private void OnEnable() => Actions.Enable();
        private void OnDisable() => Actions.Disable();
    }
}
