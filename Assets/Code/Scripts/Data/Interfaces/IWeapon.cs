using UnityEngine;

namespace Game.Data.Interfaces
{
    /// <summary>
    /// An interface describing a weapon.
    /// </summary>
    public interface IWeapon : IUsable
    {
        float Damage { get; }
        float Range { get; }
    }
}
