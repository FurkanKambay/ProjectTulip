using System;
using Game.Data.Items;
using UnityEngine;

namespace Game.Data
{
    public class ItemStack
    {
        public bool IsValid => Item != null && Amount > 0;

        public Item Item { get; }

        private int amount;
        public int Amount
        {
            get => amount;
            set => amount = Mathf.Clamp(value, 0, Item ? Item.MaxAmount : 0);
        }

        public ItemStack(Item item, int amount = 1)
        {
            Item = item;
            Amount = amount;
        }

        public ItemStack(ScriptableObject so, int amount = 1)
        {
            if (so is null)
                throw new ArgumentNullException(nameof(so));
            if (so is not Item item)
                throw new ArgumentException($"ScriptableObject {so.name} is not an IItem", nameof(so));

            Item = item;
            Amount = amount;
        }
    }
}
