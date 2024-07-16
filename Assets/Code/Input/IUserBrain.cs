namespace Tulip.Input
{
    public interface IUserBrain
    {
        public bool WantsToMenu { get; }
        public bool WantsToCancel { get; }
        public int? TabSwitchDelta { get; }
    }
}
