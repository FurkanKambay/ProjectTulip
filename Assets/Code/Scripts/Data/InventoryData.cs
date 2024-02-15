using System;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Inventory Data", menuName = "Data/Starting Inventory")]
    public class InventoryData : ScriptableObject, IValidate
    {
        public int Capacity => capacity;
        public ItemStack[] Inventory => inventory;

        [SerializeField] int capacity = 9;
        [SerializeField] ItemStack[] inventory;

        public void OnValidate()
        {
            if (inventory.Length > capacity)
                Array.Resize(ref inventory, capacity);
        }
    }
}
