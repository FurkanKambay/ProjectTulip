using System;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Interaction
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Interactable : MonoBehaviour
    {
        [SerializeField] BoxCollider2D trigger;

        public event Action OnInteract;

        private bool isHovering;

        private void HandleInteract(InputAction.CallbackContext context)
        {
            if (!isHovering) return;
            OnInteract?.Invoke();
        }

        // TODO: highlight sprite outline
        private void Update() => isHovering = trigger.OverlapPoint(InputHelper.Instance.MouseWorldPoint);

        private void OnEnable() => InputHelper.Actions.Player.Interact.performed += HandleInteract;
        private void OnDisable() => InputHelper.Actions.Player.Interact.performed -= HandleInteract;

        private void OnValidate()
        {
            if (!trigger || !trigger.isTrigger)
                Debug.LogError("Interaction trigger box is invalid!", trigger);
        }
    }
}
