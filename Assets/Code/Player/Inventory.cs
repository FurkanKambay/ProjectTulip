using System;
using System.Linq;
using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Player
{
    public sealed class Inventory : InventoryBase
    {
        // TODO: provide affected indexes
        public override event Action OnModify;

        [Header("Config")]
        [SerializeField] InventoryData inventoryData;
        [SerializeField, Min(0)] int capacity = 9;

        public override int Capacity => capacity;
        public override ItemStack[] Items { get; protected set; }

        private void Awake()
        {
            ItemStack[] startingInventory = inventoryData.Inventory.ToArray();
            Array.Resize(ref startingInventory, capacity);

            Items = startingInventory;
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
            int removeRemainder = RemoveItem(modification.ToRemove);
            int addRemainder = AddItem(modification.ToAdd);

            var stackNotRemoved = new ItemStack(modification.ToRemove.item, removeRemainder);
            var stackNotAdded = new ItemStack(modification.ToAdd.item, addRemainder);

            return !modification.WouldModify
                ? InventoryModification.Empty
                : new InventoryModification(
                    modification.WouldRemove ? stackNotRemoved : default,
                    modification.WouldAdd ? stackNotAdded : default
                );
        }

        private int RemoveItem(ItemStack itemStack)
        {
            if (!itemStack.IsValid)
                return 0;

            int remaining = itemStack.Amount;

            while (remaining > 0)
            {
                // TODO: first remove from selected hotbar slot
                int? foundIndex = GetFirstSlotWith(itemStack.item, intentToRemove: true);

                if (!foundIndex.HasValue)
                    break;

                ItemStack foundStack = Items[foundIndex.Value];
                int oldAmount = foundStack.Amount;

                int newAmount = Items[foundIndex.Value].Amount -= remaining;
                remaining -= oldAmount - newAmount;
            }

            if (remaining != itemStack.Amount)
                OnModify?.Invoke();

            return remaining;
        }

        private int AddItem(ItemStack itemStack)
        {
            if (!itemStack.IsValid)
                return 0;

            int remaining = itemStack.Amount;

            while (remaining > 0)
            {
                int? foundIndex = GetFirstSlotWith(itemStack.item, intentToRemove: false);

                if (!foundIndex.HasValue)
                {
                    foundIndex = CreateNewStackWith(itemStack.item);

                    if (!foundIndex.HasValue)
                    {
                        // Inventory is full
                        break;
                    }
                }

                remaining = AddToExistingSlot(foundIndex.Value, remaining);
            }

            if (remaining != itemStack.Amount)
                OnModify?.Invoke();

            return remaining;
        }

        private int AddToExistingSlot(int stackIndex, int amount)
        {
            ItemStack stack = Items[stackIndex];
            int wouldTotal = stack.Amount + amount;

            Items[stackIndex].Amount += amount;

            bool hasOverflow = wouldTotal > stack.item.MaxAmount;
            int overflowAmount = wouldTotal - stack.item.MaxAmount;
            return hasOverflow ? overflowAmount : 0;
        }

        private int? CreateNewStackWith(Item item)
        {
            int? firstEmptyIndex = GetFirstEmptySlot();

            if (!firstEmptyIndex.HasValue)
                return null;

            Items[firstEmptyIndex.Value] = new ItemStack(item, 0);
            return firstEmptyIndex;
        }

        private int? GetFirstSlotWith(Item item, bool intentToRemove)
        {
            if (!item)
                return null;

            for (int itemIndex = 0; itemIndex < Items.Length; itemIndex++)
            {
                ItemStack currentItem = Items[itemIndex];

                if (currentItem.item != item)
                    continue;

                // don't allow full stacks when adding
                if (!intentToRemove && currentItem.Amount >= item.MaxAmount)
                    continue;

                // don't allow empty stacks when removing
                if (intentToRemove && currentItem.Amount < 1)
                    continue;

                return itemIndex;
            }

            return null;
        }

        private int? GetFirstEmptySlot()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (!Items[i].item)
                    return i;
            }

            return null;
        }
    }
}
