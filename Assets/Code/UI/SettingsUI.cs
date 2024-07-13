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

        // ReSharper disable UnusedMember.Local
        [CreateProperty] bool IsQuitConfirmButtonVisible => IsInMainMenu && ShouldShowQuitButton;
        [CreateProperty] bool IsSaveExitButtonVisible => !IsInMainMenu && ShouldShowQuitButton;
        // ReSharper restore UnusedMember.Local

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

#if UNITY_WEBGL
            resolutionDropdown.RemoveFromHierarchy();
#endif
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
            // TODO: rewrite this in a better way
            if (GameState.Current == GameState.MainMenu)
            {
                // same as <cancel.action.triggered> in Main Menu
                if (menu.action.triggered)
                    optionsButton.value = !optionsButton.value;
            }
            else
            {
                if (menu.action.triggered)
                    optionsButton.value = true;

                if (cancel.action.triggered)
                    optionsButton.value = false;
            }

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
