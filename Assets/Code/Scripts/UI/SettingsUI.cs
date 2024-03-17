using Tulip.Core;
using Tulip.Input;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public class SettingsUI : MonoBehaviour
    {
        public UnityEvent onShow;
        public UnityEvent onHide;
        public UnityEvent onClickExit;

        [SerializeField] AudioSource audioSource;

        [CreateProperty]
        public Visibility QuitConfirmButtonVisibility
            => IsInMainMenu && ShouldShowQuitButton ? Visibility.Visible : Visibility.Hidden;

        [CreateProperty]
        public Visibility SaveExitButtonVisibility
            => !IsInMainMenu && ShouldShowQuitButton ? Visibility.Visible : Visibility.Hidden;

        private static bool IsInMainMenu => GameState.Current == GameState.MainMenu;
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
            UIDocument document = GetComponent<UIDocument>();
            document.enabled = true;
            root = document.rootVisualElement;

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
            GameState.SetPaused(change.newValue);

            if (change.newValue)
                onShow?.Invoke();
            else
                onHide?.Invoke();
        }

        private void HandlePause(InputAction.CallbackContext context)
            => optionsButton.value = true;

        private void HandleResume(InputAction.CallbackContext context)
            => optionsButton.value = GameState.Current == GameState.MainMenu && !optionsButton.value;

        private void HandleGameStateChange()
        {
            root.visible = GameState.Current != GameState.Playing;
        }

        private async void HandleSaveExitClicked(ClickEvent _)
        {
            SaveGame();
            quitFlyoutButton.value = false;
            await GameState.SwitchTo(GameState.MainMenu);
            optionsButton.value = false;
            onClickExit?.Invoke();
        }

        private void HandleQuitClicked(ClickEvent _) => GameState.QuitGame();

        // TODO: save game
        private void SaveGame() => Debug.Log("Saving...");

        private void OnEnable()
        {
            root.visible = true;
            container.visible = false;

            GameState.OnGameStateChange += HandleGameStateChange;
            InputHelper.Instance.Actions.Player.Menu.performed += HandlePause;
            InputHelper.Instance.Actions.UI.Cancel.performed += HandleResume;
            InputHelper.Instance.Actions.UI.SwitchTab.performed += HandleTabSwitch;
        }

        private void OnDisable()
        {
            root.visible = false;
            container.visible = false;

            GameState.OnGameStateChange -= HandleGameStateChange;
        }
    }
}
