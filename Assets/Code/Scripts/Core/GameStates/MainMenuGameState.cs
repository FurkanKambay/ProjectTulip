using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tulip.Core
{
    public class MainMenuGameState : GameState
    {
        protected override Awaitable Activate()
        {
            if (SceneManager.GetSceneByName("Game").isLoaded)
                SceneManager.UnloadSceneAsync("Game");

            return Awaitable.FromAsyncOperation(SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Additive));
        }

        protected override Awaitable Deactivate()
            => Awaitable.FromAsyncOperation(SceneManager.UnloadSceneAsync("Main Menu"));

        private void OnEnable() => MainMenu = this;
    }
}
