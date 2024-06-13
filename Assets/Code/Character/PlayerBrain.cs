using System;
using SaintsField;
using Tulip.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Character
{
    public class PlayerBrain : MonoBehaviour, IDasherBrain, IJumperBrain, IWielderBrain
    {
        public event Action OnJump;
        public event Action OnJumpReleased;

        [Header("References")]
        [SerializeField, Required] SaintsInterface<Component, IHealth> health;

        [Header("Input")]
        [SerializeField] InputActionReference point;
        [SerializeField] InputActionReference move;
        [SerializeField] InputActionReference jump;
        [SerializeField] InputActionReference dash;
        [SerializeField] InputActionReference use;

        public float HorizontalMovement { get; private set; }
        public bool WantsToDash { get; private set; }

        public bool WantsToUse { get; private set; }
        public Vector3 AimPosition { get; private set; }

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

            if (jump.action.triggered)
                OnJump?.Invoke();
            else if (jump.action.WasReleasedThisFrame())
                OnJumpReleased?.Invoke();
        }
    }
}