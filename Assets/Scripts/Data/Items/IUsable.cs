using Game.Data.Tiles;
using UnityEngine;

namespace Game.Data.Items
{
    public interface IUsable
    {
        [Tooltip("Time in seconds between uses")]
        float UseTime { get; }
        Sprite Icon { get; }
        float IconScale { get; }

        bool CanUseOnBlock(BlockTile block);
    }
}
