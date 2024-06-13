using System;
using System.Linq;
using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Player
{
    public sealed class Inventory : InventoryBase
    {
        public override ItemStack[] Items { get; protected set; }
        public override int Capacity => capacity;

        public override ItemStack HotbarSelected => this[HotbarSelectedIndex];
        public override int HotbarSelectedIndex
        {
            get => hotbarSelectedIndex;
            protected set => hotbarSelectedIndex = Mathf.Clamp(value, 0, Items.Length - 1);
        }

        [Header("Input")]
        [SerializeField] InputActionReference scroll;
        [SerializeField] InputActionReference hotbar;

        [Header("Config")]
        [SerializeField] InventoryData inventoryData;
        [SerializeField, Min(0)] int capacity = 9;

        private int hotbarSelectedIndex;

        public override event Action<int> OnChangeHotbarSelection;
        public override event Action OnModifyHotbar;

        public override ItemStack this[int index] => index < Items?.Length ? Items?[index] : null;

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
        {
            if (itemStack is not { IsValid: true })
                return 0;

            int remainingAmount = itemStack.Amount;
            while (remainingAmount > 0)
            {
                int foundIndex = HotbarSelected.Item == itemStack.Item
                    ? HotbarSelectedIndex
                    : FindFirstItemIndex(itemStack.Item, includeFullStacks: true);

                if (foundIndex < 0)
                    break;

                ItemStack foundStack = Items[foundIndex];
                int oldAmount = foundStack.Amount;
                foundStack.Amount -= remainingAmount;
                remainingAmount -= oldAmount - foundStack.Amount;

                if (foundStack.Amount == 0)
                    Items[foundIndex] = null;
            }

            OnModifyHotbar?.Invoke();
            return remainingAmount;
        }

        private int AddItem(ItemStack itemStack)
        {
            if (itemStack is not { IsValid: true })
                return 0;

            int remainingAmount = itemStack.Amount;
            while (remainingAmount > 0)
            {
                int foundIndex = FindFirstItemIndex(itemStack.Item, includeFullStacks: false);
                if (foundIndex < 0)
                {
                    foundIndex = CreateNewStack(itemStack.Item);

                    if (foundIndex < 0)
                    {
                        // Inventory is full
                        return itemStack.Amount;
                    }
                }

                remainingAmount = AddToExistingStack(foundIndex, remainingAmount);
            }

            if (remainingAmount == 0)
                OnModifyHotbar?.Invoke();

            return remainingAmount;
        }

        private int AddToExistingStack(int stackIndex, int amount)
        {
            ItemStack existingStack = Items[stackIndex];
            existingStack.Amount += amount;

            int total = existingStack.Amount + amount;
            bool hasOverflow = total > existingStack.Item.MaxAmount;
            int remainingAmount = total - existingStack.Item.MaxAmount;

            return hasOverflow ? remainingAmount : 0;
        }

        private int CreateNewStack(Item item)
        {
            int firstEmptyIndex = FindFirstEmptyIndex();
            if (firstEmptyIndex < 0) return -1;

            Items[firstEmptyIndex] = new ItemStack(item, 0);
            return firstEmptyIndex;
        }

        private int FindFirstItemIndex(Item item, bool includeFullStacks = false)
        {
            if (item == null) return -1;

            for (int itemIndex = 0; itemIndex < Items.Length; itemIndex++)
            {
                ItemStack currentItem = Items[itemIndex];
                if (currentItem == null) continue;

                if (currentItem.Item == item && (includeFullStacks || currentItem.Amount < item.MaxAmount))
                    return itemIndex;
            }

            return -1;
        }

        private int FindFirstEmptyIndex()
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
            ItemStack[] startingInventory = inventoryData.Inventory.Select(stack => new ItemStack(stack)).ToArray();
            Array.Resize(ref startingInventory, capacity);

            Items = startingInventory;

            OnModifyHotbar?.Invoke();
            OnChangeHotbarSelection?.Invoke(HotbarSelectedIndex);
        }

        private void Update()
        {
            if (scroll.action.inProgress)
            {
                float delta = scroll.action.ReadValue<float>();
                if (delta != 0)
                    HandleHotbarSelected(HotbarSelectedIndex - Math.Sign(delta));
            }

            if (hotbar.action.inProgress)
            {
                int i = (int)hotbar.action.ReadValue<float>();
                HandleHotbarSelected(i);
            }
        }
    }
}
