using System;
using UnityEngine;

namespace Game.Data.Interfaces
{
    public interface IInventory
    {
        ItemStack[] Items { get; }
        ItemStack this[int index] { get; set; }

        ItemStack HotbarSelected { get; }
        int HotbarSelectedIndex { get; }

        event Action OnModifyHotbar;
        event Action<int> OnChangeHotbarSelection;
    }

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
