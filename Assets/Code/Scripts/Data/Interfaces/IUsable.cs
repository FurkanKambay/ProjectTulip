namespace Game.Data.Interfaces
{
    /// <summary>
    /// An interface describing a basic item that can be used.
    /// </summary>
    public interface IUsable : IItem
    {
        float Cooldown { get; }
        float ChargeTime { get; }
        float SwingTime { get; }
    }
}
