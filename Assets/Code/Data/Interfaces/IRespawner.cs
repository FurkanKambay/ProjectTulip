namespace Tulip.Data
{
    public interface IRespawner
    {
        float SecondsUntilRespawn { get; }
        bool CanRespawn { get; }

        void TryRespawn();
    }
}
