namespace Tulip.Core
{
    public class LoadingGameState : GameState
    {
        public GameState From { get; private set; }
        public GameState To { get; private set; }

        public LoadingGameState With(GameState from, GameState to)
        {
            From = from;
            To = to;
            return this;
        }

        public override bool RequestApplicationQuit() => false;

        protected override void OnEnable() => Loading = this;
    }
}
