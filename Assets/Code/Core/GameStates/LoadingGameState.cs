namespace Tulip.Core
{
    public class LoadingGameState : GameState
    {
        public override bool IsPlayerInputEnabled => false;

        public GameState From { get; set; }
        public GameState To { get; set; }

        public LoadingGameState With(GameState from, GameState to)
        {
            From = from;
            To = to;
            return this;
        }

        protected override bool CanQuitGame() => false;
        private void OnEnable() => Loading = this;
    }
}
