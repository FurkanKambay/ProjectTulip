using Game.Data.Tiles;

namespace Game.Data.Interfaces
{
    public interface ITool : IUsable
    {
        bool CanUseOnBlock(BlockTile block);
    }
}
