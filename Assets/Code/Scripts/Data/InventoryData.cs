using System;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Inventory Data", menuName = "Data/Starting Inventory")]
    public class InventoryData : ScriptableObject, IValidate
    {
        public int Capacity => capacity;
        public ScriptableObject[] Inventory => inventory;

        [SerializeField] private int capacity = 9;
        [SerializeField] private ScriptableObject[] inventory;

        public void OnValidate()
        {
            inventory ??= new ScriptableObject[capacity];
            if (inventory.Length != capacity)
                Array.Resize(ref inventory, capacity);
        }
    }
}
