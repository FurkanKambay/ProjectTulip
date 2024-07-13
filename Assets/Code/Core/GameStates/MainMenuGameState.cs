namespace Tulip.Core
{
    public class MainMenuGameState : GameState
    {
        public override bool IsPlayerInputEnabled => true;
        public override bool IsUIInputEnabled => true;

        private void OnEnable() => MainMenu = this;
    }
}
