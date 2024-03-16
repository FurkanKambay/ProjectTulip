using System;
using UnityEditor;
using UnityEngine;

namespace Tulip.Core
{
    public class GameState : ScriptableObject
    {
        public static event Action OnGameStateChange;

        private static GameState Empty { get; set; }
        protected static LoadingGameState Loading { get; set; }
        public static MainMenuGameState MainMenu { get; protected set; }
        public static PlayingGameState Playing { get; protected set; }
        public static PausedGameState Paused { get; protected set; }

        private static GameState currentState;
        public static GameState Current
        {
            get => (bool)currentState ? currentState : Empty;
            private set
            {
                if (currentState == value) return;
                currentState = value;
                OnGameStateChange?.Invoke();
            }
        }

        public static async Awaitable<GameState> SwitchTo(GameState newState)
        {
            if (newState == Current) return Current;
            Debug.Log($"[Game State] Switching from {Current} to {newState}.");

            GameState oldState = Current;
            Current = Loading.With(oldState, newState);

            await oldState.Deactivate();
            await newState.Activate();

            Current = newState;
            return Current;
        }

        public virtual bool IsPlayerInputEnabled => false;

        protected virtual Awaitable Activate() => Awaitable.EndOfFrameAsync();
        protected virtual Awaitable Deactivate() => Awaitable.EndOfFrameAsync();
        protected virtual void TrySetPaused(bool paused) { }
        protected virtual bool CanQuitGame() => true;

        public static void SetPaused(bool paused) => Current.TrySetPaused(paused);

        public static void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static async void Init()
        {
            Debug.Log("Reset game state.");
            currentState = Empty;
            await SwitchTo(MainMenu);
        }

        public override string ToString() => $"|{name[13..]}|";

        private void OnEnable()
        {
            Empty = this;
            Application.wantsToQuit += () => Current.CanQuitGame();
        }
    }
}
