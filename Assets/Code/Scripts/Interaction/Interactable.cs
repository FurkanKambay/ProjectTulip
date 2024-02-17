using System;
using Tulip.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Interaction
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Interactable : MonoBehaviour
    {
        [SerializeField] BoxCollider2D trigger;

        public event Action OnInteract;

        private Camera mainCamera;

        private bool isHovering;

        private void HandleInteract(InputAction.CallbackContext context)
        {
            if (!isHovering) return;
            OnInteract?.Invoke();
        }

        private void Awake() => mainCamera = Camera.main;

        // TODO: highlight sprite outline
        private void Update()
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(InputHelper.Instance.MouseScreenPoint);
            isHovering = trigger.OverlapPoint(mouseWorld);
        }

        private void OnEnable() => InputHelper.Actions.Player.Interact.performed += HandleInteract;
        private void OnDisable() => InputHelper.Actions.Player.Interact.performed -= HandleInteract;

        private void OnValidate()
        {
            if (!trigger || !trigger.isTrigger)
                Debug.LogError("Interaction trigger box is invalid!", trigger);
        }
    }
}
