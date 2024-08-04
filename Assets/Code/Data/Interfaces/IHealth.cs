using Tulip.Data.Gameplay;

namespace Tulip.Data
{
    public interface IHealth
    {
        public delegate void DamageEvent(HealthChangeEventArgs damage);
        public delegate void DeathEvent(HealthChangeEventArgs damage);
        public delegate void HealEvent(HealthChangeEventArgs healing);
        public delegate void ReviveEvent(IHealth reviver);

        event DamageEvent OnHurt;
        event DeathEvent OnDie;
        event HealEvent OnHeal;
        event ReviveEvent OnRevive;

        float CurrentHealth { get; }
        float MaxHealth { get; }
        float Ratio { get; }
        bool IsAlive { get; }
        bool IsDead { get; }
        bool IsFull { get; }
        bool IsHurt { get; }
        bool IsInvulnerable { get; }

        ITangibleEntity Entity { get; }
        string Name { get; }
        IHealth LatestDamageSource { get; }
        IHealth LatestDeathSource { get; }

        InventoryModification Damage(float amount, IHealth source, bool checkInvulnerable);
        void Heal(float amount, IHealth source);
        void Revive(IHealth reviver = null);
    }
}
