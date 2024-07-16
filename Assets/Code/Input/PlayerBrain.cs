using System;
using SaintsField;
using Tulip.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Input
{
    public class PlayerBrain : MonoBehaviour, IPlayerBrain
    {
        public event Action OnJump;
        public event Action OnJumpReleased;

        [Header("References")]
        [SerializeField, Required] HealthBase health;

        [Header("Input - Basic")]
        [SerializeField, Required] InputActionReference point;
        [SerializeField, Required] InputActionReference move;
        [SerializeField, Required] InputActionReference jump;
        [SerializeField, Required] InputActionReference dash;
        [SerializeField, Required] InputActionReference use;

        [Header("Input - Misc")]
        [SerializeField, Required] InputActionReference zoom;
        [SerializeField, Required] InputActionReference smartCursor;
        [SerializeField, Required] InputActionReference hotbarScroll;
        [SerializeField, Required] InputActionReference hotbar;

        public Vector2 AimPosition { get; private set; }
        public float HorizontalMovement { get; private set; }
        public bool WantsToDash { get; private set; }
        public bool WantsToUse { get; private set; }

        public float ZoomDelta { get; private set; }
        public bool WantsToToggleSmartCursor { get; private set; }
        public int HotbarSelectionDelta { get; private set; }
        public int? HotbarSelectionIndex { get; private set; }

        private Camera mainCamera;

        private void Awake() => mainCamera = Camera.main;

        private void Update()
        {
            if (!health || health.IsDead)
            {
                HorizontalMovement = default;
                WantsToDash = false;
                WantsToUse = false;

                WantsToToggleSmartCursor = false;
                HotbarSelectionDelta = 0;
                HotbarSelectionIndex = null;

                OnJumpReleased?.Invoke();
                return;
            }

            AimPosition = mainCamera.ScreenToWorldPoint(point.action.ReadValue<Vector2>());
            HorizontalMovement = move.action.ReadValue<float>();
            WantsToDash = dash.action.inProgress;
            WantsToUse = use.action.inProgress;

            ZoomDelta = zoom.action.ReadValue<float>();
            WantsToToggleSmartCursor = smartCursor.action.triggered;
            HotbarSelectionDelta = Math.Sign(hotbarScroll.action.ReadValue<float>());
            HotbarSelectionIndex = !hotbar.action.inProgress ? null : (int)hotbar.action.ReadValue<float>();

            if (jump.action.triggered)
                OnJump?.Invoke();
            else if (jump.action.WasReleasedThisFrame())
                OnJumpReleased?.Invoke();
        }
    }
}
