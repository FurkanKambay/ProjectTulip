namespace Tulip.Core
{
    public class MainMenuGameState : GameState
    {
        public override bool IsPlayerInputEnabled => false;

        private void OnEnable() => MainMenu = this;
    }
}
