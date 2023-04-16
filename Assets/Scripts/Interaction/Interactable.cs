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

        public event Action Interacted;

        private InputHelper input;
        private bool isHovering;

        private void OnClick(InputAction.CallbackContext context)
        {
            if (!isHovering) return;
            Interacted?.Invoke();
        }

        private void Awake() => input = InputHelper.Instance;

        // TODO: highlight sprite outline
        private void Update() => isHovering = trigger.OverlapPoint(input.MouseWorldPoint);

        private void OnEnable() => InputHelper.Actions.Player.Use.performed += OnClick;
        private void OnDisable() => InputHelper.Actions.Player.Use.performed -= OnClick;

        private void OnValidate()
        {
            if (!trigger || !trigger.isTrigger)
                Debug.LogError("Interaction trigger box is invalid!", trigger);
        }
    }
}
