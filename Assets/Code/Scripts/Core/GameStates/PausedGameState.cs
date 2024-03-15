using UnityEngine;

namespace Tulip.Core
{
    public class PausedGameState : PlayingGameState
    {
        public override bool IsPlayerInputEnabled => false;

        protected override Awaitable Activate()
        {
            Time.timeScale = Options.Instance.Gameplay.AllowPause ? 0 : 1;
            return Awaitable.EndOfFrameAsync();
        }

        protected override Awaitable Deactivate() => Awaitable.EndOfFrameAsync();

        protected override async void TrySetPaused(bool paused)
        {
            if (!paused) await SwitchTo(Playing);
        }

        private void OnEnable() => Paused = this;
    }
}
