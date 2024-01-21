using Game.Data.Interfaces;
using Game.Data.Tiles;
using UnityEngine;

namespace Game.Data.Items
{
    /// <summary>
    /// An item that can be used on a tile.
    /// </summary>
    [CreateAssetMenu(fileName = "Tool", menuName = "Items/Tool")]
    public class Tool : Usable, ITool
    {
        public virtual bool CanUseOnBlock(BlockTile block) => false;
    }
}
