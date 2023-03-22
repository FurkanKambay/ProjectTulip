using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class Input : MonoBehaviour
    {
        public static Input Instance { get; private set; }
        public static InputActions Actions => Instance.actions ?? (Instance.actions = new InputActions());

        public Action<int> HotbarSelected;
        public Vector2 MouseWorldPoint => mainCamera.ScreenToWorldPoint(screenPoint);

        private InputActions actions;
        private Camera mainCamera;
        private Vector2 screenPoint;

        private void OnHotbar(InputAction.CallbackContext context)
            => HotbarSelected?.Invoke(Convert.ToInt32(context.control.name) - 1);

        private void OnPoint(InputAction.CallbackContext context)
            => screenPoint = context.ReadValue<Vector2>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            mainCamera = Camera.main;
            Actions.Player.Hotbar.performed += OnHotbar;
            Actions.Player.Point.performed += OnPoint;
        }

        private void OnEnable() => Actions.Enable();
        private void OnDisable() => Actions.Disable();
    }
}
