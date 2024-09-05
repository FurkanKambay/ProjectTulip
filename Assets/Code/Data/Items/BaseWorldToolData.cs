using UnityEngine;

namespace Tulip.Data.Items
{
    /// <summary>
    /// An item that can be used on a tile.
    /// </summary>
    public abstract class BaseWorldToolData : UsableData
    {
        public abstract ToolUsability GetUsability(IWorld world, Vector2Int cell);
        public abstract InventoryModification UseOn(IWorld world, Vector2Int cell);
    }

    public enum ToolUsability
    {
        /// Cell is out of world bounds
        Never,
        /// A different tile exists on the cell
        Invalid,
        /// An entity is blocking the cell temporarily
        NotNow,
        /// Cell already has the same tile (or is already empty)
        NoEffect,
        /// Cell is available
        Available,
    }
}
