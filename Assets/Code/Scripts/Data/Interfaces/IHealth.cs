using System;
using Tulip.Data.Gameplay;

namespace Tulip.Data
{
    public interface IHealth
    {
        event Action<DamageEventArgs> OnHurt;
        event Action<DamageEventArgs> OnDie;

        float CurrentHealth { get; }
        float MaxHealth { get; }
        float Ratio => CurrentHealth / MaxHealth;
        bool IsAlive => CurrentHealth > 0;
        bool IsDead => CurrentHealth <= 0;
        bool IsFull => CurrentHealth >= MaxHealth;
        bool IsHurt => CurrentHealth < MaxHealth && !IsDead;
    }
}
