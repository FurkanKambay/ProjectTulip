using UnityEditor;
using UnityEngine;

namespace Tulip.Core
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Testing
    }

    public class GameManager : MonoBehaviour
    {
        public delegate void GameStateChangeEvent(GameState oldState, GameState newState);

        public static event GameStateChangeEvent OnGameStateChange;

        public static GameState CurrentState { get; private set; }
        public static bool IsPlayerInputEnabled => CurrentState != GameState.Paused;
        public static bool IsUIInputEnabled => CurrentState == GameState.MainMenu || !IsPlayerInputEnabled;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            Debug.Log("[Game State] Initializing.");
            CurrentState = GameState.MainMenu;
            // no need to invoke the event or to update timeScale
        }

        private void OnEnable() => Application.wantsToQuit += IsSafeToQuit;
        private void OnDisable() => Application.wantsToQuit -= IsSafeToQuit;

        public static void SwitchTo(GameState newState)
        {
            if (newState == CurrentState)
                return;

            GameState oldState = CurrentState;
            Debug.Log($"[Game State] Switching from {oldState} to {newState}");

            CurrentState = newState;

            UpdateTimeScale();
            OnGameStateChange?.Invoke(oldState, newState);
        }

        public static void SetPaused(bool shouldPause) => SwitchTo(
            CurrentState switch
            {
                GameState.Playing when shouldPause => GameState.Paused,
                GameState.Paused when !shouldPause => GameState.Playing,
                _ => CurrentState
            }
        );

        public static void QuitGame()
        {
            if (!IsSafeToQuit())
                return;

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private static void UpdateTimeScale() => Time.timeScale = CurrentState switch
        {
            GameState.Paused when Options.Instance.Gameplay.AllowPause => 0,
            _ => 1
        };

        private static bool IsSafeToQuit()
        {
            if (CurrentState is not (GameState.Playing or GameState.Paused))
                return true;

            // TODO: save game before quitting
            Debug.LogWarning("Quit requested. Should save game first.");

            return true;
        }
    }
}
