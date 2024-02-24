using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tulip
{
    public class Bootstrapper : MonoBehaviour
    {
        [CreateProperty] public static GameState GameState { get; private set; }

#if !UNITY_EDITOR
        private void Awake() => Application.wantsToQuit += HandleQuitRequested;
#endif

        private void Start() => SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Additive);

        public static void LoadGameScene()
        {
            if (GameState != GameState.InMainMenu) return;

            SceneManager.UnloadSceneAsync("Main Menu");
            SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive).completed +=
                _ => GameState = GameState.InGame;
        }

        public static void ReturnToMainMenu()
        {
            if (GameState == GameState.InMainMenu) return;

            SceneManager.UnloadSceneAsync("Game");
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Additive);
            GameState = GameState.InMainMenu;
        }

        // ReSharper disable once UnusedMember.Local
        private static bool HandleQuitRequested()
        {
            switch (GameState)
            {
                case GameState.InMainMenu:
                default:
                    return true;
                case GameState.InGame:
                case GameState.Paused:
                    ReturnToMainMenu();
                    return false;
            }
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
