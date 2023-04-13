using System;
using System.Linq;
using Game.Data;
using Game.Data.Interfaces;
using Game.Data.Items;
using Game.Gameplay;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public sealed class Inventory : InventoryBase
    {
        public override ItemStack[] Items { get; protected set; }
        public override ItemStack HotbarSelected => Items[HotbarSelectedIndex];

        private int hotbarSelectedIndex;
        public override int HotbarSelectedIndex
        {
            get => hotbarSelectedIndex;
            protected set => hotbarSelectedIndex = Mathf.Clamp(value, 0, Items.Length - 1);
        }

        public Pickaxe FirstPickaxe => Items.Select(s => s.Item).OfType<Pickaxe>().First();

        public override ItemStack this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }

        [SerializeField] HotbarData hotbarData;
        [SerializeField] LayerMask weaponHitMask;

        private WeaponWielder wielder;

        public override event Action<int> HotbarSelectionChanged;
        public override event Action HotbarModified;

        public bool RemoveItem(IItem item, int amount)
        {
            int remaining = amount;
            while (remaining > 0)
            {
                int index = HotbarSelected.Item == item
                    ? HotbarSelectedIndex
                    : GetFirstItemIndex(item, allowAny: true);

                if (index < 0)
                {
                    HotbarModified?.Invoke();
                    break;
                }

                ItemStack stack = Items[index];
                int oldAmount = stack.Amount;
                stack.Amount -= remaining;
                remaining -= oldAmount - stack.Amount;

                if (stack.Amount == 0)
                    Items[index] = null;
            }

            if (remaining > 0)
                return false;

            HotbarModified?.Invoke();
            return true;
        }

        public int AddItem(IItem item, int amount)
        {
            int remaining = amount;
            while (remaining > 0)
            {
                int index = GetFirstItemIndex(item, allowAny: false);
                if (index < 0) index = CreateNewStack(item);
                if (index < 0)
                {
                    Debug.LogWarning("Inventory full");
                    return amount;
                }

                remaining = AddToExistingStack(index, remaining);
            }

            if (remaining == 0) HotbarModified?.Invoke();
            return remaining;
        }

        private int AddToExistingStack(int stackIndex, int amount)
        {
            ItemStack stack = Items[stackIndex];
            int total = stack.Amount + amount;
            stack.Amount += amount;

            return total > stack.Item.MaxAmount ? total - stack.Item.MaxAmount : 0;
        }

        private int CreateNewStack(IItem item)
        {
            int firstEmpty = GetFirstEmptyIndex();
            if (firstEmpty < 0) return -1;

            Items[firstEmpty] = new ItemStack(item, 0);
            return firstEmpty;
        }

        private int GetFirstItemIndex(IItem item, bool allowAny = false)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i]?.Item == item && (allowAny || Items[i].Amount < item?.MaxAmount))
                    return i;
            }
            return -1;
        }

        private int GetFirstEmptyIndex()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i]?.Item is null)
                    return i;
            }
            return -1;
        }

        private void OnHotbarSelected(int index)
        {
            if (index == HotbarSelectedIndex)
                return;

            HotbarSelectedIndex = index;
            PrepareSelectedItem();
            HotbarSelectionChanged?.Invoke(HotbarSelectedIndex);
        }

        private void PrepareSelectedItem()
        {
            if (HotbarSelected?.Item is not WeaponData weapon)
            {
                wielder.enabled = false;
                return;
            }

            wielder.enabled = true;
            wielder.data = weapon;
        }

        private void Awake()
        {
            Items = hotbarData.hotbar.Select(so => so ? new ItemStack(so) : null).ToArray();
            HotbarModified?.Invoke();

            wielder = gameObject.AddComponent<WeaponWielder>();
            wielder.enabled = HotbarSelected?.Item is WeaponData;
            wielder.hitMask = weaponHitMask;
        }

        private void OnScroll(InputAction.CallbackContext context)
        {
            float delta = context.ReadValue<float>();
            if (delta == 0) return;

            OnHotbarSelected(HotbarSelectedIndex - Math.Sign(delta));
        }

        private void OnEnable()
        {
            InputHelper.Actions.Player.Scroll.performed += OnScroll;
            InputHelper.Instance.HotbarSelected += OnHotbarSelected;
            HotbarSelectionChanged?.Invoke(HotbarSelectedIndex);
        }

        private void OnDisable()
        {
            InputHelper.Actions.Player.Scroll.performed -= OnScroll;
            InputHelper.Instance.HotbarSelected -= OnHotbarSelected;
        }
    }
}
