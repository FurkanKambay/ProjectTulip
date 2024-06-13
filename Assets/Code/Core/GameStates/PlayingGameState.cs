using UnityEngine;

namespace Tulip.Core
{
    public class PlayingGameState : GameState
    {
        public override bool IsPlayerInputEnabled => true;

        protected override void Activate() => Time.timeScale = 1;

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
