using UnityEngine;

namespace Tulip.Core
{
    [CreateAssetMenu]
    public class TestingGameState : PlayingGameState
    {
        public override bool IsPlayerInputEnabled => true;

        protected override Awaitable Activate()
        {
            Time.timeScale = 1;
            return Awaitable.EndOfFrameAsync();
        }

        protected override void TrySetPaused(bool paused) { }
        protected override bool CanQuitGame() => true;
        private void OnEnable() => Testing = this;
    }
}
