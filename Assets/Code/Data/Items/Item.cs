using Unity.Properties;
using UnityEngine;

namespace Tulip.Data.Items
{
    /// <summary>
    /// A base item that can be stored in an inventory.
    /// </summary>
    [CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
    public class Item : ScriptableObject
    {
        public virtual ItemType Type => type;
        public virtual Sprite Icon => icon;
        public virtual float IconScale => iconScale;
        public virtual string Name => name;
        public virtual string Description => description;
        public virtual int MaxAmount => maxAmount;

        // ReSharper disable UnusedMember.Local
        [CreateProperty] bool CanHaveMultiple => MaxAmount > 1;
        [CreateProperty] float IconHeight => IconScale * 24f;
        // ReSharper restore UnusedMember.Local

        [Header("Item Data")]
        [SerializeField] protected ItemType type;
        [SerializeField] protected Sprite icon;
        [SerializeField] protected float iconScale = 1f;
        [SerializeField] protected new string name;
        [SerializeField, Multiline] protected string description;
        [SerializeField, Min(1)] protected int maxAmount = 1;
    }
}
