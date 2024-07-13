namespace Tulip.Core
{
    public class MainMenuGameState : GameState
    {
        public override bool IsPlayerInputEnabled => true;

        private void OnEnable() => MainMenu = this;
    }
}
