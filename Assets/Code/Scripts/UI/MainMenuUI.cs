using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Game
{
    public class MainMenuUI : MonoBehaviour
    {
        private UIDocument document;
        private VisualElement root;

        private void Awake()
        {
            document = GetComponent<UIDocument>();
            root = document.rootVisualElement.ElementAt(0);

            root.Q<Button>("PlayButton").RegisterCallback<ClickEvent>(HandlePlayClicked);
            root.Q<Button>("SettingsButton").RegisterCallback<ClickEvent>(HandleSettingsClicked);
            root.Q<Button>("QuitButton").RegisterCallback<ClickEvent>(HandleQuitClicked);
        }

        private void HandlePlayClicked(ClickEvent _) => SceneManager.LoadSceneAsync("Game");

        private void HandleSettingsClicked(ClickEvent _) => Debug.Log("Settings clicked");

        private static void HandleQuitClicked(ClickEvent _)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
