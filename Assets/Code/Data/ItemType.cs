namespace Tulip.Data
{
    public enum ItemType
    {
        WorldTile,
        Item,
        Usable,
        WorldTool,
        Weapon
    }

    public enum TileType
    {
        /// <summary>
        /// A background wall tile.
        /// </summary>
        Wall,
        /// <summary>
        /// A regular block tile in the game world.
        /// </summary>
        Block,
        /// <summary>
        /// A foreground curtain tile.
        /// </summary>
        Curtain
    }
}
