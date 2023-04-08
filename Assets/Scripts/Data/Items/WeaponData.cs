using Game.Data.Interfaces;
using UnityEngine;

namespace Game.Data.Items
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon")]
    public class WeaponData : Weapon
    {
        public bool CanMultiHit => canMultiHit;

        [SerializeField] bool canMultiHit;
    }
}
