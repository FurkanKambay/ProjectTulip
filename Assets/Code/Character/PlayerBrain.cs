using System;
using SaintsField;
using Tulip.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Character
{
    public class PlayerBrain : MonoBehaviour, IPlayerBrain, IDasherBrain, IJumperBrain, IWielderBrain
    {
        public event Action OnJump;
        public event Action OnJumpReleased;

        [Header("References")]
        [SerializeField, Required] SaintsInterface<Component, IHealth> health;

        [Header("Input - Basic")]
        [SerializeField] InputActionReference point;
        [SerializeField] InputActionReference move;
        [SerializeField] InputActionReference jump;
        [SerializeField] InputActionReference dash;
        [SerializeField] InputActionReference use;

        [Header("Input - Misc")]
        [SerializeField] InputActionReference smartCursor;
        [SerializeField] InputActionReference hotbarScroll;
        [SerializeField] InputActionReference hotbar;

        public Vector2 AimPosition { get; private set; }
        public float HorizontalMovement { get; private set; }
        public bool WantsToDash { get; private set; }
        public bool WantsToUse { get; private set; }

        public bool WantsToToggleSmartCursor { get; private set; }
        public int HotbarSelectionDelta { get; private set; }
        public int? HotbarSelectionIndex { get; private set; }

        private Camera mainCamera;

        private void Awake() => mainCamera = Camera.main;

        private void Update()
        {
            if (health.I.IsDead)
            {
                OnJumpReleased?.Invoke();
                return;
            }

            AimPosition = mainCamera.ScreenToWorldPoint(point.action.ReadValue<Vector2>());
            HorizontalMovement = move.action.ReadValue<float>();
            WantsToDash = dash.action.inProgress;
            WantsToUse = use.action.inProgress;

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
