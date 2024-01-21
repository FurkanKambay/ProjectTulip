using System;
using Game.Data.Interfaces;
using UnityEngine;

namespace Game.Data
{
    public abstract class InventoryBase : MonoBehaviour, IInventory
    {
        public abstract ItemStack[] Items { get; protected set; }
        public abstract ItemStack HotbarSelected { get; }
        public abstract int HotbarSelectedIndex { get; protected set; }

        public abstract ItemStack this[int index] { get; set; }

        public abstract event Action OnModifyHotbar;
        public abstract event Action<int> OnChangeHotbarSelection;
    }
}
