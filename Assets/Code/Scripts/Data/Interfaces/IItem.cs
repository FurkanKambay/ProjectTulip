using UnityEngine;

namespace Game.Data.Interfaces
{
    /// <summary>
    /// An interface describing a base item that can be stored in an inventory.
    /// </summary>
    public interface IItem
    {
        string Name { get; }
        string Description { get; }
        ItemType Type { get; }
        Sprite Icon { get; }
        float IconScale { get; }
        int MaxAmount { get; }
    }
}
