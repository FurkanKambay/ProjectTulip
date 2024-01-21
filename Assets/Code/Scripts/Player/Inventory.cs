using System;
using System.Linq;
using Game.Data;
using Game.Data.Interfaces;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public sealed class Inventory : InventoryBase
    {
        public override ItemStack[] Items { get; protected set; }
        public override ItemStack HotbarSelected => Items[HotbarSelectedIndex];

        public override int HotbarSelectedIndex
        {
            get => hotbarSelectedIndex;
            protected set => hotbarSelectedIndex = Mathf.Clamp(value, 0, Items.Length - 1);
        }

        [SerializeField] HotbarData hotbarData;
        private int hotbarSelectedIndex;

        public override event Action<int> OnChangeHotbarSelection;
        public override event Action OnModifyHotbar;

        public override ItemStack this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }

        /// <summary>
        /// Applies the <see cref="InventoryModification"/> to the inventory by first removing the items in
        /// <see cref="InventoryModification.ToRemove"/>, then adding items to the inventory from
        /// <see cref="InventoryModification.ToAdd"/>.
        /// </summary>
        /// <param name="modification">The item stacks to remove from and add to the inventory.</param>
        /// <returns>The remaining item stacks that could not be removed or added.</returns>
        public InventoryModification ApplyModification(InventoryModification modification)
        {
            var notRemoved = new ItemStack(modification.ToRemove?.Item, RemoveItem(modification.ToRemove));
            var notAdded = new ItemStack(modification.ToAdd?.Item, AddItem(modification.ToAdd));
            return !modification.WouldModify
                ? InventoryModification.Empty
                : new InventoryModification(
                    modification.WouldRemove ? notRemoved : null,
                    modification.WouldAdd ? notAdded : null
                );
        }

        private int RemoveItem(ItemStack itemStack)
            => itemStack is { IsValid: true } ? RemoveItem(itemStack.Item, itemStack.Amount) : 0;

        private int AddItem(ItemStack itemStack)
            => itemStack is { IsValid: true } ? AddItem(itemStack.Item, itemStack.Amount) : 0;

        private int RemoveItem(IItem item, int amount)
        {
            if (item == null || amount <= 0) return amount;

            int remaining = amount;
            while (remaining > 0)
            {
                int index = HotbarSelected.Item == item
                    ? HotbarSelectedIndex
                    : GetFirstItemIndex(item, allowAny: true);

                if (index < 0)
                {
                    OnModifyHotbar?.Invoke();
                    break;
                }

                ItemStack stack = Items[index];
                int oldAmount = stack.Amount;
                stack.Amount -= remaining;
                remaining -= oldAmount - stack.Amount;

                if (stack.Amount == 0)
                    Items[index] = null;
            }

            if (remaining == 0)
                OnModifyHotbar?.Invoke();

            return remaining;
        }

        private int AddItem(IItem item, int amount)
        {
            if (item == null || amount <= 0) return amount;

            int remaining = amount;
            while (remaining > 0)
            {
                int index = GetFirstItemIndex(item, allowAny: false);
                if (index < 0) index = CreateNewStack(item);
                if (index < 0)
                {
                    // Inventory full
                    return amount;
                }

                remaining = AddToExistingStack(index, remaining);
            }

            if (remaining == 0)
                OnModifyHotbar?.Invoke();

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

        private void HandleHotbarSelected(int index)
        {
            if (index == HotbarSelectedIndex)
                return;

            HotbarSelectedIndex = index;
            OnChangeHotbarSelection?.Invoke(HotbarSelectedIndex);
        }

        private void Awake()
        {
            Items = hotbarData.hotbar.Select(so => so ? new ItemStack(so) : null).ToArray();
            OnModifyHotbar?.Invoke();
        }

        private void OnScroll(InputAction.CallbackContext context)
        {
            float delta = context.ReadValue<float>();
            if (delta == 0) return;

            HandleHotbarSelected(HotbarSelectedIndex - Math.Sign(delta));
        }

        private void OnEnable()
        {
            InputHelper.Actions.Player.Scroll.performed += OnScroll;
            InputHelper.Instance.OnSelectHotbar += HandleHotbarSelected;
            OnChangeHotbarSelection?.Invoke(HotbarSelectedIndex);
        }

        private void OnDisable()
        {
            InputHelper.Actions.Player.Scroll.performed -= OnScroll;
            InputHelper.Instance.OnSelectHotbar -= HandleHotbarSelected;
        }
    }
}
