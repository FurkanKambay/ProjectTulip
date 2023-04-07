using System;
using System.Linq;
using Game.Data;
using Game.Data.Interfaces;
using Game.Data.Items;
using Game.Gameplay;
using UnityEngine;

namespace Game.Player
{
    public class Inventory : MonoBehaviour
    {
        public IItem[] Items { get; private set; }
        public IItem HotbarSelected => Items[HotbarSelectedIndex];
        public int HotbarSelectedIndex { get; private set; }
        public Pickaxe FirstPickaxe => Items.OfType<Pickaxe>().First();

        [SerializeField] private HotbarData hotbarData;
        [SerializeField] private LayerMask weaponHitMask;

        private MeleeWeapon meleeWeapon;

        public event Action<int> HotbarSelectionChanged;
        public event Action HotbarModified;

        private void OnHotbarSelected(int index)
        {
            if (index == HotbarSelectedIndex)
                return;

            HotbarSelectedIndex = index;
            PrepareSelectedItem();

            HotbarSelectionChanged?.Invoke(index);
        }

        private void PrepareSelectedItem()
        {
            if (HotbarSelected is not WeaponData weapon)
            {
                meleeWeapon.enabled = false;
                return;
            }

            meleeWeapon.enabled = true;
            meleeWeapon.data = weapon;
        }

        private void Awake()
        {
            Items = hotbarData.hotbar.Cast<IItem>().ToArray();
            HotbarModified?.Invoke();

            meleeWeapon = gameObject.AddComponent<MeleeWeapon>();
            meleeWeapon.enabled = HotbarSelected is WeaponData;
            meleeWeapon.hitMask = weaponHitMask;
        }

        private void OnEnable()
        {
            Input.Instance.HotbarSelected += OnHotbarSelected;
            HotbarSelectionChanged?.Invoke(HotbarSelectedIndex);
        }

        private void OnDisable() => Input.Instance.HotbarSelected -= OnHotbarSelected;
    }
}
