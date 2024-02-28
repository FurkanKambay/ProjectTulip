using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tulip.Core
{
    public class Bootstrapper : MonoBehaviour
    {
        public static event Action OnGameStateChange;

        public static GameState GameState
        {
            get => gameState;
            private set
            {
                if (gameState == value) return;
                gameState = value;
                OnGameStateChange?.Invoke();
            }
        }

        private static GameState gameState;

#if !UNITY_EDITOR
        private void Awake() => Application.wantsToQuit += HandleQuitRequested;
#endif

        private void Start()
        {
            GameState = GameState.InMainMenu;
            SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Additive);
        }

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

        public static void TrySetGamePaused(bool paused)
        {
            if (gameState == GameState.InMainMenu || !Options.Game.AllowPause)
            {
                Time.timeScale = 1;
                return;
            }

            GameState = paused ? GameState.Paused : GameState.InGame;
            Time.timeScale = paused ? 0 : 1;
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
