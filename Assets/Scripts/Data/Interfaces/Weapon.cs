using UnityEngine;

namespace Game.Data.Interfaces
{
    public abstract class Weapon : Usable
    {
        public float Damage => damage;
        public float Range => range;

        [Header("Weapon Data")]
        [SerializeField] float damage = 1f;
        [SerializeField] float range = 1f;
    }
}
