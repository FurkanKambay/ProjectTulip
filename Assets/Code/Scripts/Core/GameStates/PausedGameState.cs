namespace Tulip.Core
{
    public class PausedGameState : PlayingGameState
    {
        protected override void OnEnable() => Paused = this;
    }
}
