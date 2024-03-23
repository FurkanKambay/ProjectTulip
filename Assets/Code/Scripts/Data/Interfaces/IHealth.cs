using Tulip.Data.Gameplay;

namespace Tulip.Data
{
    public interface IHealth
    {
        public delegate void DamageEvent(DamageEventArgs damage);
        public delegate void DeathEvent(DamageEventArgs death);
        public delegate void ReviveEvent(IHealth source);

        event DamageEvent OnHurt;
        event DeathEvent OnDie;
        event ReviveEvent OnRevive;

        float CurrentHealth { get; }
        float MaxHealth { get; }

        float Ratio => CurrentHealth / MaxHealth;
        bool IsAlive => CurrentHealth > 0;
        bool IsDead => CurrentHealth <= 0;
        bool IsFull => CurrentHealth >= MaxHealth;
        bool IsHurt => CurrentHealth < MaxHealth && !IsDead;
    }
}
