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
        float Ratio { get; }
        bool IsAlive { get; }
        bool IsDead { get; }
        bool IsFull { get; }
        bool IsHurt { get; }
        bool IsInvulnerable { get; }

        string Name { get; }
        IHealth LatestDamageSource { get; }
        IHealth LatestDeathSource { get; }

        void TakeDamage(float damage, IHealth source);
        void Revive(IHealth source = null);
    }
}
