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

            settingsUI = FindFirstObjectByType<SettingsUI>();

            playButton = root.Q<Button>("PlayButton");
            quitButton = root.Q<Button>("QuitButton");

            InputHelper.Actions.Player.Disable();
        }

        private void OnEnable()
        {
            playButton.RegisterCallback<ClickEvent>(HandlePlayClicked);
            quitButton.RegisterCallback<ClickEvent>(HandleQuitClicked);
        }

        private void OnDisable()
        {
            playButton.UnregisterCallback<ClickEvent>(HandlePlayClicked);
            quitButton.UnregisterCallback<ClickEvent>(HandleQuitClicked);
        }

        private void HandlePlayClicked(ClickEvent _)
        {
            SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync("Main Menu");
        }

        private static void HandleQuitClicked(ClickEvent _)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
