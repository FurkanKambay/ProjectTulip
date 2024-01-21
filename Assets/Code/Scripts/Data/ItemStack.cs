using System;
using Game.Data.Interfaces;
using UnityEngine;

namespace Game.Data
{
    public class ItemStack
    {
        public bool IsValid => Item != null && Amount > 0;

        public IItem Item { get; }

        private int amount;
        public int Amount
        {
            get => amount;
            set => amount = Mathf.Clamp(value, 0, Item?.MaxAmount ?? 0);
        }

        public ItemStack(IItem item, int amount = 1)
        {
            Item = item;
            Amount = amount;
        }

        public ItemStack(ScriptableObject so, int amount = 1)
        {
            if (so is null)
                throw new ArgumentNullException(nameof(so));
            if (so is not IItem item)
                throw new ArgumentException($"ScriptableObject {so.name} is not an IItem", nameof(so));

            Item = item;
            Amount = amount;
        }
    }
}
