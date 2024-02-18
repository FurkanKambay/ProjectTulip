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
        private Button settingsButton;
        private Button quitButton;

        private void Awake()
        {
            document = GetComponent<UIDocument>();
            root = document.rootVisualElement.ElementAt(0);

            settingsUI = FindObjectOfType<SettingsUI>();

            playButton = root.Q<Button>("PlayButton");
            settingsButton = root.Q<Button>("SettingsButton");
            quitButton = root.Q<Button>("QuitButton");

            InputHelper.Actions.Player.Disable();
        }

        private void OnEnable()
        {
            playButton.RegisterCallback<ClickEvent>(HandlePlayClicked);
            settingsButton.RegisterCallback<ClickEvent>(HandleSettingsClicked);
            quitButton.RegisterCallback<ClickEvent>(HandleQuitClicked);
        }

        private void OnDisable()
        {
            playButton.UnregisterCallback<ClickEvent>(HandlePlayClicked);
            settingsButton.UnregisterCallback<ClickEvent>(HandleSettingsClicked);
            quitButton.UnregisterCallback<ClickEvent>(HandleQuitClicked);
        }

        private void HandlePlayClicked(ClickEvent _)
        {
            SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync("Main Menu");
        }

        private void HandleSettingsClicked(ClickEvent _) => settingsUI.enabled = true;

        private static void HandleQuitClicked(ClickEvent _)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
