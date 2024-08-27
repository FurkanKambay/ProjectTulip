using UnityEngine;

namespace Tulip.Data.Items
{
    /// <summary>
    /// An item that can be used on a tile.
    /// </summary>
    public abstract class BaseWorldToolData : UsableData
    {
        public abstract bool IsUsableOn(IWorld world, Vector2Int cell);
        public abstract InventoryModification UseOn(IWorld world, Vector2Int cell);
    }
}
