using UnityEngine;

namespace Game.Data.Interfaces
{
    public interface IItem
    {
        string Name { get; }
        string Description { get; }
        ItemType Type { get; }
        Sprite Icon { get; }
        float IconScale { get; }
        int MaxAmount { get; }
    }

    public enum ItemType
    {
        Item,
        Block,
        Wall,
        Pickaxe,
        Weapon
    }
}
