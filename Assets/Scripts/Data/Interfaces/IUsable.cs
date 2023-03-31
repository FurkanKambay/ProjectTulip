namespace Game.Data.Interfaces
{
    public interface IUsable : IItem
    {
        float Cooldown { get; }
    }
}
