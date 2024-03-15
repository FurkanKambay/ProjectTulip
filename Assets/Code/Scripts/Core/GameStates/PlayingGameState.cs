using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tulip.Core
{
    public class PlayingGameState : GameState
    {
        public override bool IsPlayerInputEnabled => true;

        protected override Awaitable Activate()
        {
            Time.timeScale = 1;

            if (SceneManager.GetSceneByName("Game").isLoaded)
                return base.Activate();

            AsyncOperation operation = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
            return Awaitable.FromAsyncOperation(operation);
        }

        protected override async void TrySetPaused(bool paused)
        {
            if (paused)
                await SwitchTo(Paused);
        }

        protected override bool CanQuitGame()
        {
            // TODO: save game, then quit
            Debug.LogWarning("Force quit requested. Should save game first.");
            return true;
        }

        private void OnEnable() => Playing = this;
    }
}
