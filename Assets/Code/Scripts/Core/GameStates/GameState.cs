using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

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
        public static TestingGameState Testing { get; protected set; }

        private static bool isTransitioning;
        private static GameState currentState;

        public static GameState Current
        {
            get => (bool)currentState ? currentState : currentState = Empty;
            private set
            {
                if (currentState == value) return;
                currentState = value;
                OnGameStateChange?.Invoke();
            }
        }

        public static async Awaitable SwitchTo(GameState newState)
        {
            Assert.IsNotNull(newState);
            if (newState == Current) return;

            bool hasWarned = false;
            int frameWaitCount = 0;

            while (isTransitioning)
            {
                if (!hasWarned)
                {
                    Debug.LogWarning($"[Game State] Can't switch from {Current} to {newState} now. Waiting.");
                    hasWarned = true;
                }

                await Awaitable.NextFrameAsync();
                frameWaitCount++;
            }

            isTransitioning = true;

            string extraMessage = !hasWarned ? "." : $" after waiting for {frameWaitCount} frame(s).";
            string logMessage = $"[Game State] Switching from {Current} to {newState}";
            Debug.Log(logMessage + extraMessage);

            GameState oldState = Current;
            Current = Loading.With(oldState, newState);

            await oldState.Deactivate();
            await newState.Activate();

            Current = newState;
            isTransitioning = false;
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
            Debug.Log("[Game State] Initializing.");
            currentState = Empty;

            if (currentState == null)
                Debug.LogError("[Game State] Empty game state is null.");

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
