using UnityEngine;

namespace Game.Data.Interfaces
{
    /// <summary>
    /// A base item that can be stored in an inventory.
    /// </summary>
    [CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
    public class Item : ScriptableObject, IItem
    {
        public string Name => name;
        public string Description => description;
        public ItemType Type => type;
        public Sprite Icon => icon;
        public float IconScale => iconScale;
        public int MaxAmount => maxAmount;

        [Header("Item Data")]
        [SerializeField] new string name;
        [SerializeField, Multiline] string description;
        [SerializeField] ItemType type;
        [SerializeField] Sprite icon;
        [SerializeField] float iconScale = 1f;
        [SerializeField, Min(1)] int maxAmount = 1;
    }
}
