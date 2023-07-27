using UnityEngine;

namespace Game.Data.Interfaces
{
    public interface IUsable : IItem
    {
        float Cooldown { get; }
    }

    public abstract class Usable : Item, IUsable
    {
        public float Cooldown => cooldown;

        [Header("Usable Data")]
        [SerializeField, Min(0)] float cooldown = .5f;
    }
}
