using UnityEngine;

namespace Game.Data.Interfaces
{
    public interface IItem
    {
        Sprite Icon { get; }
        float IconScale { get; }
        int MaxAmount { get; }
    }

    public abstract class Item : ScriptableObject, IItem
    {
        public Sprite Icon => icon;
        public float IconScale => iconScale;
        public int MaxAmount => maxAmount;

        [Header("Item Data")]
        [SerializeField] Sprite icon;
        [SerializeField] float iconScale = 1f;
        [SerializeField, Min(1)] int maxAmount = 1;
    }
}
