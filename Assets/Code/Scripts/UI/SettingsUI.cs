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

        private VisualElement root;
        private Toggle toggleButton;
        private TabView tabView;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            root = GetComponent<UIDocument>().rootVisualElement;
            toggleButton = root.Q<Toggle>("ToggleButton");
            tabView = root.Q<TabView>();
            tabView.visible = false;

            toggleButton.RegisterCallback<ChangeEvent<bool>>(HandleToggle);
        }

        private void HandleToggle(ChangeEvent<bool> change)
        {
            tabView.visible = change.newValue;

            if (change.newValue)
            {
                OnShow?.Invoke();
                InputHelper.Actions.UI.Cancel.performed += HandleEscape;
            }
            else
            {
                OnHide?.Invoke();
                InputHelper.Actions.UI.Cancel.performed -= HandleEscape;
            }
        }

        private void HandleEscape(InputAction.CallbackContext context) => toggleButton.value = false;

        private void OnEnable() => root.visible = true;
        private void OnDisable() => root.visible = false;
    }
}
