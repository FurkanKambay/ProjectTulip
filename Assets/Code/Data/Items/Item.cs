using SaintsField;
using UnityEngine;

namespace Tulip.Data.Items
{
    /// <summary>
    /// A base item that can be stored in an inventory.
    /// </summary>
    [CreateAssetMenu(menuName = "Items/Item")]
    public class Item : ScriptableObject
    {
        public virtual ItemType Type => type;
        public virtual Sprite Icon => icon;
        public virtual float IconScale => iconScale;
        public virtual string Name => name;
        public virtual string Description => description;
        public virtual int MaxAmount => maxAmount;

        [Header("Item Data")]
        [SerializeField] protected ItemType type;

        [AssetPreview(width: 64, align: EAlign.FieldStart)]
        [SerializeField] protected Sprite icon;

        [SerializeField] protected float iconScale = 1f;
        [SerializeField] protected new string name;
        [SerializeField, Multiline] protected string description;
        [SerializeField, Min(1)] protected int maxAmount = 1;

        public override string ToString() => Name;
    }
}
