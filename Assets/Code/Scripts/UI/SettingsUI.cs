using System;
using Tulip.Core;
using Tulip.Input;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public class SettingsUI : MonoBehaviour
    {
        [SerializeField] private InputHelper inputHelper;

        public event Action OnShow;
        public event Action OnHide;

        [SerializeField] AudioSource audioSource;

        [CreateProperty]
        public Visibility QuitConfirmButtonVisibility
            => IsInMainMenu && ShouldShowQuitButton ? Visibility.Visible : Visibility.Hidden;

        [CreateProperty]
        public Visibility SaveExitButtonVisibility
            => !IsInMainMenu && ShouldShowQuitButton ? Visibility.Visible : Visibility.Hidden;

        private static bool IsInMainMenu => Bootstrapper.GameState == GameState.MainMenu;
        private bool ShouldShowQuitButton => container.visible && quitFlyoutButton.value;

        private VisualElement root;
        private VisualElement container;
        private TabView tabView;

        private Toggle optionsButton;
        private Toggle quitFlyoutButton;
        private Button menuQuitButton;
        private Button gameExitButton;
        private DropdownField resolutionDropdown;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            root = GetComponent<UIDocument>().rootVisualElement;

            container = root.Q<VisualElement>("MainContainer");
            container.visible = false;
            container.dataSource = this;

            tabView = root.Q<TabView>();
            optionsButton = root.Q<Toggle>("OptionsButton");
            quitFlyoutButton = root.Q<Toggle>("QuitFlyoutButton");
            gameExitButton = root.Q<Button>("SaveExitButton");
            menuQuitButton = root.Q<Button>("QuitConfirmButton");
            resolutionDropdown = root.Q<DropdownField>("VideoResolution");

            optionsButton.RegisterCallback<ChangeEvent<bool>>(HandleOptionsToggle);
            gameExitButton.RegisterCallback<ClickEvent>(HandleSaveExitClicked);
            menuQuitButton.RegisterCallback<ClickEvent>(HandleQuitClicked);

            // UXML binding does not work
            resolutionDropdown.choices = Options.Instance.Video.SupportedResolutions;
        }

        private void HandleTabSwitch(InputAction.CallbackContext context)
            => tabView.selectedTabIndex += (int)context.ReadValue<float>();

        private void HandleOptionsToggle(ChangeEvent<bool> change)
        {
            container.visible = change.newValue;
            quitFlyoutButton.value = false;

            audioSource.Play();
            Bootstrapper.TrySetGamePaused(change.newValue);

            if (change.newValue)
                OnShow?.Invoke();
            else
                OnHide?.Invoke();
        }

        private void HandlePause(InputAction.CallbackContext context)
            => optionsButton.value = true;

        private void HandleResume(InputAction.CallbackContext context)
            => optionsButton.value = Bootstrapper.GameState == GameState.MainMenu && !optionsButton.value;

        private void HandleGameStateChange()
            => root.visible = Bootstrapper.GameState != GameState.Playing;

        private void HandleSaveExitClicked(ClickEvent _)
        {
            SaveGame();
            quitFlyoutButton.value = false;
            Bootstrapper.ReturnToMainMenu();
            optionsButton.value = false;
        }

        private void HandleQuitClicked(ClickEvent _) => Bootstrapper.QuitGame();

        // TODO: save game
        private void SaveGame() => Debug.Log("Saving...");

        private void OnEnable()
        {
            root.visible = true;
            container.visible = false;

            Bootstrapper.OnGameStateChange += HandleGameStateChange;
            inputHelper.Actions.Player.Menu.performed += HandlePause;
            inputHelper.Actions.UI.Cancel.performed += HandleResume;
            inputHelper.Actions.UI.SwitchTab.performed += HandleTabSwitch;
        }

        private void OnDisable()
        {
            root.visible = false;
            container.visible = false;

            Bootstrapper.OnGameStateChange -= HandleGameStateChange;
        }
    }
}
