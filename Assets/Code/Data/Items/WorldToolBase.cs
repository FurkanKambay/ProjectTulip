using UnityEngine;

namespace Tulip.Data.Items
{
    /// <summary>
    /// An item that can be used on a tile.
    /// </summary>
    public abstract class WorldToolBase : Usable
    {
        public abstract bool IsUsableOn(IWorld world, Vector3Int cell);
        public abstract InventoryModification UseOn(IWorld world, Vector3Int cell);
    }
}