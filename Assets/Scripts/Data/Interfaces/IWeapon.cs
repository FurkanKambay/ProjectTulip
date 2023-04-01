namespace Game.Data.Interfaces
{
    public interface IWeapon : IUsable
    {
        float Damage { get; }
        float Range { get; }
    }
}
