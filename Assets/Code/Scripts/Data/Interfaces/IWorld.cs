using System;
using Tulip.Data.Items;
using Tulip.Data.Tiles;
using UnityEngine;

namespace Tulip.Data
{
    public interface IWorld
    {
        event Action<TileModification> OnPlaceTile;
        event Action<TileModification> OnHitTile;
        event Action<TileModification> OnDestroyTile;

        /// <summary>
        /// Damages a tile at the given cell coordinates.
        /// </summary>
        /// <returns>Whether the tile was destroyed.</returns>
        InventoryModification DamageTile(Vector3Int cell, int damage);

        /// <summary>
        /// Tries to place a tile at the given cell coordinates.
        /// </summary>
        /// <returns>Whether the tile was placed successfully.</returns>
        InventoryModification PlaceTile(Vector3Int cell, WorldTile worldTile);

        bool CellIntersects(Vector3Int cell, Bounds other);
        bool HasTile(Vector3Int cell);
        WorldTile GetTile(Vector3Int cell);
        WorldTile GetTile(Vector3 worldPosition);
    }
}
