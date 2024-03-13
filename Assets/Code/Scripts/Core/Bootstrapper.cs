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
            gameState = GameState.Loading;
            OnGameStateChange?.Invoke();

            SceneManager.LoadScene("Main Menu", LoadSceneMode.Additive);
            GameState = GameState.InMainMenu;
        }

        private void OnEnable() => Options.OnUpdate += HandleOptionsUpdated;
        private void OnDisable() => Options.OnUpdate -= HandleOptionsUpdated;

        private void HandleOptionsUpdated()
        {
            string[] resolutionParts = Options.Instance.Video.Resolution.Split('\u00d7', 2);
            int width = int.Parse(resolutionParts[0]);
            int height = int.Parse(resolutionParts[1]);
            Screen.SetResolution(width, height, Options.Instance.Video.FullScreenMode);
        }

        public static void LoadGameScene()
        {
            if (GameState != GameState.InMainMenu) return;

            GameState = GameState.Loading;
            SceneManager.UnloadSceneAsync("Main Menu");
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
            GameState = GameState.InGame;
        }

        public static void ReturnToMainMenu()
        {
            if (GameState == GameState.InMainMenu) return;

            GameState = GameState.Loading;
            SceneManager.UnloadSceneAsync("Game");
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Additive);
            GameState = GameState.InMainMenu;
        }

        public static void TrySetGamePaused(bool paused)
        {
            if (gameState == GameState.InMainMenu || !Options.Instance.Gameplay.AllowPause)
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
                case GameState.Loading:
                    return false;
                case GameState.InMainMenu:
                default:
                    return true;
                case GameState.InGame:
                case GameState.Paused:
                    // TODO: save game, then quit
                    Debug.LogWarning("Force quit requested. Should save game first.");
                    return true;
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
