using System;
using Game.Data.Items;
using UnityEngine;

namespace Game.Data
{
    [Serializable]
    public class ItemStack : IValidate
    {
        public bool IsValid => Item != null && amount > 0;

        [field: SerializeField] public Item Item { get; private set; }

        [SerializeField, Min(0)] int amount;
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

        public ItemStack(ItemStack other)
        {
            Item = other.Item;
            Amount = other.Amount;
        }

        public void OnValidate() => Amount = amount;
    }
}
