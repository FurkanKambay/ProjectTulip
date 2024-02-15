using Game.Data.Items;

namespace Game.Data.Interfaces
{
    /// <summary>
    /// An interface describing an item that can be used on a tile.
    /// </summary>
    public interface ITool : IUsable
    {
        bool IsUsableOnTile(WorldTile tile);
    }
}
