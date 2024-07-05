using System;
using Tulip.Data.Items;
using Unity.Properties;
using UnityEngine;

namespace Tulip.Data
{
    [Serializable]
    public class ItemStack : IValidate
    {
        [SerializeField] Item item;
        [SerializeField, Min(0)] int amount;

        public Item Item => item;
        public int Amount
        {
            get => amount;
            set => amount = Mathf.Clamp(value, 0, Item ? Item.MaxAmount : 0);
        }

        [CreateProperty]
        public bool IsValid => Item && amount > 0;

        public ItemStack(Item item, int amount)
        {
            this.item = item;
            Amount = amount;
        }

        public ItemStack(ItemStack other)
        {
            item = other.Item;
            Amount = other.Amount;
        }

        public void OnValidate() => Amount = amount;
    }
}
