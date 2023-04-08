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

        [SerializeField] HotbarData hotbarData;
        [SerializeField] LayerMask weaponHitMask;

        private WeaponWielder wielder;

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
                wielder.enabled = false;
                return;
            }

            wielder.enabled = true;
            wielder.data = weapon;
        }

        private void Awake()
        {
            Items = hotbarData.hotbar.Cast<IItem>().ToArray();
            HotbarModified?.Invoke();

            wielder = gameObject.AddComponent<WeaponWielder>();
            wielder.enabled = HotbarSelected is WeaponData;
            wielder.hitMask = weaponHitMask;
        }

        private void OnEnable()
        {
            Input.Instance.HotbarSelected += OnHotbarSelected;
            HotbarSelectionChanged?.Invoke(HotbarSelectedIndex);
        }

        private void OnDisable() => Input.Instance.HotbarSelected -= OnHotbarSelected;
    }
}
