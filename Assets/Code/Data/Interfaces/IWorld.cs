using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    public interface IWorld
    {
        public delegate void WorldTileEvent(TileModification modification);

        event WorldTileEvent OnPlaceTile;
        event WorldTileEvent OnHitTile;
        event WorldTileEvent OnDestroyTile;

        /// <summary>
        /// Damages a tile of the given type at the given cell coordinates.
        /// </summary>
        /// <returns>Whether the tile was destroyed.</returns>
        InventoryModification DamageTile(Vector3Int cell, TileType tileType, int damage);

        /// <summary>
        /// Tries to place a tile at the given cell coordinates.
        /// </summary>
        /// <returns>Whether the tile was placed successfully.</returns>
        InventoryModification PlaceTile(Vector3Int cell, WorldTile worldTile);

        bool CellIntersects(Vector3Int cell, Bounds other);
        bool HasTile(Vector3Int cell);
        WorldTile GetTile(Vector3Int cell);
        WorldTile GetTile(Vector3 worldPosition);

        bool CanAccommodate(Vector3Int cell, Vector2Int entitySize);
    }
}
