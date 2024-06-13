using SaintsField;
using Tulip.Core;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public class SettingsUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] UIDocument document;
        [SerializeField, Required] AudioSource audioSource;

        [Header("Input")]
        [SerializeField] InputActionReference menu;
        [SerializeField] InputActionReference cancel;
        [SerializeField] InputActionReference switchTab;

        [Header("Config")]
        public UnityEvent onShow;
        public UnityEvent onHide;
        public UnityEvent onClickExit;

        // ReSharper disable UnusedMember.Global
        [CreateProperty]
        public Visibility QuitConfirmButtonVisibility => (IsInMainMenu && ShouldShowQuitButton).ToVisibility();

        [CreateProperty]
        public Visibility SaveExitButtonVisibility => (!IsInMainMenu && ShouldShowQuitButton).ToVisibility();
        // ReSharper restore UnusedMember.Global

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

        private void OnEnable()
        {
            root.visible = true;
            container.visible = false;

            GameState.OnGameStateChange += HandleGameStateChange;
        }

        private void OnDisable()
        {
            root.visible = false;
            container.visible = false;

            GameState.OnGameStateChange -= HandleGameStateChange;
        }

        private void Update()
        {
            if (menu.action.triggered)
                optionsButton.value = true;

            if (cancel.action.triggered)
                optionsButton.value = GameState.Current == GameState.MainMenu && !optionsButton.value;

            if (switchTab.action.triggered)
                tabView.selectedTabIndex += (int)switchTab.action.ReadValue<float>();
        }

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
    }
}
