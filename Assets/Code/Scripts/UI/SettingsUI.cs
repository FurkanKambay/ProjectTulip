using System;
using Tulip.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public class SettingsUI : MonoBehaviour
    {
        public event Action OnShow;
        public event Action OnHide;

        private UIDocument document;
        private VisualElement root;
        private Button backButton;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            document = GetComponent<UIDocument>();
            root = document.rootVisualElement.ElementAt(0);
            backButton = root.Q<Button>("BackButton");

            enabled = false;
        }

        private void HandleBackClicked(ClickEvent _) => enabled = false;
        private void HandleEscape(InputAction.CallbackContext context) => enabled = false;

        private void OnEnable()
        {
            root.visible = true;

            InputHelper.Actions.UI.Cancel.performed += HandleEscape;
            backButton.RegisterCallback<ClickEvent>(HandleBackClicked);

            OnShow?.Invoke();
        }

        private void OnDisable()
        {
            root.visible = false;

            InputHelper.Actions.UI.Cancel.performed -= HandleEscape;
            backButton.UnregisterCallback<ClickEvent>(HandleBackClicked);

            OnHide?.Invoke();
        }
    }
}
