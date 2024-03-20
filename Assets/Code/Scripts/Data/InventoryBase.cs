using System;
using UnityEngine;

namespace Tulip.Data
{
    public abstract class InventoryBase : MonoBehaviour, IInventory
    {
        public abstract ItemStack[] Items { get; protected set; }
        public abstract int Capacity { get; }

        public abstract ItemStack HotbarSelected { get; }
        public abstract int HotbarSelectedIndex { get; protected set; }

        public abstract ItemStack this[int index] { get; }

        public abstract event Action OnModifyHotbar;
        public abstract event Action<int> OnChangeHotbarSelection;
    }
}
