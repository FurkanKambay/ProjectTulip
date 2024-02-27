using System;
using Tulip.Core;
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

        [SerializeField] AudioSource audioSource;

        private VisualElement root;
        private VisualElement container;
        private Toggle toggleButton;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            root = GetComponent<UIDocument>().rootVisualElement;
            toggleButton = root.Q<Toggle>("ToggleButton");
            container = root.Q<VisualElement>("MainContainer");
            container.visible = false;

            root.Q<Tab>("TabGame").dataSource = Options.Game;
            root.Q<Tab>("TabSound").dataSource = Options.Sound;

            toggleButton.RegisterCallback<ChangeEvent<bool>>(HandleToggle);
        }

        private void HandleToggle(ChangeEvent<bool> change)
        {
            container.visible = change.newValue;
            audioSource.Play();

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
