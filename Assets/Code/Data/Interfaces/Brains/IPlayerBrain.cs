namespace Tulip.Data
{
    public interface IPlayerBrain
    {
        public bool WantsToToggleSmartCursor { get; }
        public int HotbarSelectionDelta { get; }
        public int? HotbarSelectionIndex { get; }
    }
}
