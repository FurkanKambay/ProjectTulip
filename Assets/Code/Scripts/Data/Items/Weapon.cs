using UnityEngine;

namespace Game.Data.Items
{
    /// <summary>
    /// A weapon.
    /// </summary>
    [CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon")]
    public class Weapon : Usable
    {
        public virtual float Damage => damage;
        public virtual float Range => range;
        public virtual bool IsMultiTarget => isMultiTarget;

        [Header("Weapon Data")]
        [SerializeField, Min(0)] protected float damage = 1f;
        [SerializeField, Min(0)] protected float range = 1f;
        [SerializeField] protected bool isMultiTarget;
    }
}
