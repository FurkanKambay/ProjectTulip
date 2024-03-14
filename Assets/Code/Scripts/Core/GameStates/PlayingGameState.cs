using UnityEngine;

namespace Tulip.Core
{
    public class PlayingGameState : GameState
    {
        public override bool RequestApplicationQuit()
        {
            // TODO: save game, then quit
            Debug.LogWarning("Force quit requested. Should save game first.");
            return true;
        }

        protected override void OnEnable() => Playing = this;
    }
}
