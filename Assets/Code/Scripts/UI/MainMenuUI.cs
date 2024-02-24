using Tulip.Input;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        private UIDocument document;
        private VisualElement root;

        private SettingsUI settingsUI;

        private Button playButton;
        private Button quitButton;

        private void Awake()
        {
            document = GetComponent<UIDocument>();
            root = document.rootVisualElement.ElementAt(0);

            settingsUI = FindAnyObjectByType<SettingsUI>();

            playButton = root.Q<Button>("PlayButton");
            quitButton = root.Q<Button>("QuitButton");

            InputHelper.Actions.Player.Disable();
        }

        private void OnEnable()
        {
            settingsUI.OnShow += HandleSettingsShow;
            settingsUI.OnHide += HandleSettingsHide;

            playButton.RegisterCallback<ClickEvent>(HandlePlayClicked);
            quitButton.RegisterCallback<ClickEvent>(HandleQuitClicked);
        }

        private void OnDisable()
        {
            settingsUI.OnShow -= HandleSettingsShow;
            settingsUI.OnHide -= HandleSettingsHide;

            playButton.UnregisterCallback<ClickEvent>(HandlePlayClicked);
            quitButton.UnregisterCallback<ClickEvent>(HandleQuitClicked);
        }

        private void HandleSettingsShow() => root.visible = false;
        private void HandleSettingsHide() => root.visible = true;

        private void HandlePlayClicked(ClickEvent _) => Bootstrapper.LoadGameScene();
        private static void HandleQuitClicked(ClickEvent _) => Bootstrapper.QuitGame();
    }
}
