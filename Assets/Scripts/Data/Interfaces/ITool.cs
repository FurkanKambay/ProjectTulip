using Game.Data.Tiles;

namespace Game.Data.Interfaces
{
    public interface ITool : IUsable
    {
        bool CanUseOnBlock(BlockTile block);
    }

    public abstract class Tool : Usable, ITool
    {
        public virtual bool CanUseOnBlock(BlockTile block) => block;
    }
}
