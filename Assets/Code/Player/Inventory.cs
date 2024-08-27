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
        /// Applies the <see cref="InventoryModification"/> to the inventory.
        /// </summary>
        /// <param name="modification">The item stack to remove from / add to the inventory.</param>
        /// <returns>The remaining item stack.</returns>
        public InventoryModification ApplyModification(InventoryModification modification)
        {
            if (!modification.IsValid)
                return default;

            int remainder = modification.WouldAdd
                ? AddItem(modification.Stack)
                : RemoveItem(modification.Stack);

            ItemStack remainingStack = modification.Stack.itemData.Stack(remainder);

            return modification.WouldAdd
                ? InventoryModification.ToAdd(remainingStack)
                : InventoryModification.ToRemove(remainingStack);
        }

        private int RemoveItem(ItemStack itemStack)
        {
            if (!itemStack.IsValid)
                return 0;

            int remaining = itemStack.Amount;

            while (remaining > 0)
            {
                // TODO: first remove from selected hotbar slot
                int? foundIndex = GetFirstSlotWith(itemStack.itemData, intentToRemove: true);

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
                int? foundIndex = GetFirstSlotWith(itemStack.itemData, intentToRemove: false);

                if (!foundIndex.HasValue)
                {
                    foundIndex = CreateNewStackWith(itemStack.itemData);

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

            bool hasOverflow = wouldTotal > stack.itemData.MaxAmount;
            int overflowAmount = wouldTotal - stack.itemData.MaxAmount;
            return hasOverflow ? overflowAmount : 0;
        }

        private int? CreateNewStackWith(ItemData itemData)
        {
            int? firstEmptyIndex = GetFirstEmptySlot();

            if (!firstEmptyIndex.HasValue)
                return null;

            Items[firstEmptyIndex.Value] = new ItemStack(itemData, 0);
            return firstEmptyIndex;
        }

        private int? GetFirstSlotWith(ItemData itemData, bool intentToRemove)
        {
            if (!itemData)
                return null;

            for (int itemIndex = 0; itemIndex < Items.Length; itemIndex++)
            {
                ItemStack currentItem = Items[itemIndex];

                if (currentItem.itemData != itemData)
                    continue;

                // don't allow full stacks when adding
                if (!intentToRemove && currentItem.Amount >= itemData.MaxAmount)
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
                if (!Items[i].itemData)
                    return i;
            }

            return null;
        }
    }
}
