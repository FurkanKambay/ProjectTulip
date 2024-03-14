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

        private void Awake()
        {
            gameState = GameState.Loading.With(GameState.Empty, GameState.MainMenu);

            SceneManager.LoadScene("Main Menu", LoadSceneMode.Additive);
            GameState = GameState.MainMenu;

            Application.wantsToQuit += () => GameState.RequestApplicationQuit();
        }

        private void OnEnable() => Options.OnUpdate += HandleOptionsUpdated;
        private void OnDisable() => Options.OnUpdate -= HandleOptionsUpdated;

        private static void HandleOptionsUpdated()
        {
            string[] resolutionParts = Options.Instance.Video.Resolution.Split('\u00d7', 2);
            int width = int.Parse(resolutionParts[0]);
            int height = int.Parse(resolutionParts[1]);
            Screen.SetResolution(width, height, Options.Instance.Video.FullScreenMode);
        }

        public static void LoadGameScene()
        {
            if (GameState != GameState.MainMenu) return;

            GameState = GameState.Loading.With(gameState, GameState.Playing);
            SceneManager.UnloadSceneAsync("Main Menu");

            SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive)
                .completed += _ =>
            {
                // TODO: set the World and Player here if possible
                GameState = GameState.Playing;
            };
        }

        public static void ReturnToMainMenu()
        {
            if (GameState == GameState.MainMenu) return;

            GameState = GameState.Loading.With(gameState, GameState.MainMenu);
            SceneManager.UnloadSceneAsync("Game");

            SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Additive)
                .completed += _ => GameState = GameState.MainMenu;
        }

        public static void TrySetGamePaused(bool paused)
        {
            if (gameState == GameState.MainMenu || !Options.Instance.Gameplay.AllowPause)
            {
                Time.timeScale = 1;
                return;
            }

            GameState = paused ? GameState.Paused : GameState.Playing;
            Time.timeScale = paused ? 0 : 1;
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
