using UnityEngine;

namespace Game.Data.Items
{
    /// <summary>
    /// An item that can be used on a tile.
    /// </summary>
    [CreateAssetMenu(fileName = "Tool", menuName = "Items/Tool")]
    public class Tool : Usable
    {
        public virtual bool IsUsableOnTile(WorldTile tile) => false;
    }
}
