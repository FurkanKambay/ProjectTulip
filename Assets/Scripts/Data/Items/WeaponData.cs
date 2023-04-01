using Game.Data.Interfaces;
using UnityEngine;

namespace Game.Data.Items
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon")]
    public class WeaponData : ScriptableObject, IWeapon
    {
        public float Damage => damage;
        public float Range => range;
        public float Cooldown => cooldown;

        public Sprite Icon => icon;
        public float IconScale => iconScale;

        [SerializeField] private Sprite icon;
        [SerializeField] private float iconScale = 1f;

        [Header("Data")]
        [SerializeField] private float damage = 1f;
        [SerializeField] private float range = 1f;
        [SerializeField] private float cooldown = .5f;
    }
}
