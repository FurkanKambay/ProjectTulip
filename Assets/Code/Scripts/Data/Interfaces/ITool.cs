using Game.Data.Tiles;

namespace Game.Data.Interfaces
{
    /// <summary>
    /// An interface describing an item that can be used on a tile.
    /// </summary>
    public interface ITool : IUsable
    {
        bool CanUseOnTile(WorldTile tile);
    }
}
