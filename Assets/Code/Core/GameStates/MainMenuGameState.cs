using UnityEngine;

namespace Tulip.Core
{
    public class MainMenuGameState : GameState
    {
        public override bool IsPlayerInputEnabled => true;
        public override bool IsUIInputEnabled => true;

        protected override void Activate() => Time.timeScale = 1;

        private void OnEnable() => MainMenu = this;
    }
}
