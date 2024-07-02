using System;
using UnityEngine;

namespace Tulip.Data
{
    public abstract class InventoryBase : MonoBehaviour, IInventory
    {
        public abstract event Action OnModify;

        public abstract int Capacity { get; }

        public abstract ItemStack[] Items { get; protected set; }
        public abstract ItemStack this[int index] { get; }
    }
}
