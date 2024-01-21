namespace Game.Data.Interfaces
{
    public interface IHealth
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        float Ratio => CurrentHealth / MaxHealth;
        bool IsAlive => CurrentHealth > 0;
        bool IsDead => CurrentHealth <= 0;
        bool IsFull => CurrentHealth >= MaxHealth;
        bool IsHurt => CurrentHealth < MaxHealth && !IsDead;
    }
}
