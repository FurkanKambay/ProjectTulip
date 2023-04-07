using UnityEngine;

namespace Game.Data.Interfaces
{
    public interface IItem
    {
        Sprite Icon { get; }
        float IconScale { get; }
    }

    public abstract class Item : ScriptableObject, IItem
    {
        public Sprite Icon => icon;
        public float IconScale => iconScale;

        [Header("Item Data")]
        [SerializeField] private Sprite icon;
        [SerializeField] private float iconScale = 1f;
    }
}
