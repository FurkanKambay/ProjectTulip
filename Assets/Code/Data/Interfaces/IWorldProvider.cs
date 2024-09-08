namespace Tulip.Data
{
    public interface IWorldProvider
    {
        public delegate void ProvideWorldEvent(WorldData worldData);

        event ProvideWorldEvent OnProvideWorld;

        public WorldData World { get; }
    }
}
