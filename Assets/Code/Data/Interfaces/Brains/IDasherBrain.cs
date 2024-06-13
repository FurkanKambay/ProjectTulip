namespace Tulip.Data
{
    public interface IDasherBrain : IWalkerBrain
    {
        public bool WantsToDash { get; }
    }
}
