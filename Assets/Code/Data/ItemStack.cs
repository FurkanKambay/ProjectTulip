using System;
using SaintsField;
using Tulip.Data.Items;
using Unity.Properties;
using UnityEngine;

namespace Tulip.Data
{
    [Serializable]
    public struct ItemStack
    {
        public Item item;
        public bool isLocked;

        [Min(0), MaxValue(nameof(MaxAmount))]
        [SerializeField] int amount;

        public int MaxAmount => item ? item.MaxAmount : 0;

        public int Amount
        {
            get => amount;
            set
            {
                amount = Mathf.Clamp(value, 0, MaxAmount);

                if (amount == 0 && !isLocked)
                    item = null;
            }
        }

        [CreateProperty]
        public bool IsValid => item && amount > 0;

        // ReSharper disable UnusedMember.Local
        [CreateProperty] bool ShowAmount => MaxAmount > 1;
        [CreateProperty] bool ShowIcon => isLocked || IsValid;
        [CreateProperty] float IconHeight => item ? item.IconScale * 24f : 0f;
        [CreateProperty] float IconOpacity => isLocked && amount == 0 ? 0.5f : 1f;
        // ReSharper restore UnusedMember.Local

        public ItemStack(Item item, int amount) : this()
        {
            this.item = item;
            this.amount = Mathf.Clamp(amount, 0, MaxAmount);
        }

        public ItemStack(ItemStack other) : this(other.item, other.Amount) { }

        public override string ToString() => $"{Amount} {item}";
    }
}
