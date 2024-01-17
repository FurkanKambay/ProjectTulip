using UnityEngine;

namespace Game.Data.Interfaces
{
    public interface IItem
    {
        string Name { get; }
        string Description { get; }
        Sprite Icon { get; }
        float IconScale { get; }
        int MaxAmount { get; }
    }

    public abstract class Item : ScriptableObject, IItem
    {
        public string Name => name;
        public string Description => description;
        public Sprite Icon => icon;
        public float IconScale => iconScale;
        public int MaxAmount => maxAmount;

        [Header("Item Data")]
        [SerializeField] new string name;
        [SerializeField, Multiline] string description;
        [SerializeField] Sprite icon;
        [SerializeField] float iconScale = 1f;
        [SerializeField, Min(1)] int maxAmount = 1;
    }
}
