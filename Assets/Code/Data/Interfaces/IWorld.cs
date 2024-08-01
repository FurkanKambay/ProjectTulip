using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    public interface IWorld
    {
        public delegate void PlaceableEvent(TileModification modification);

        event PlaceableEvent OnPlaceTile;
        event PlaceableEvent OnHitTile;
        event PlaceableEvent OnDestroyTile;

        /// <summary>
        /// Damages a tile of the given type at the given cell coordinates.
        /// </summary>
        /// <returns>Whether the tile was destroyed.</returns>
        InventoryModification DamageTile(Vector3Int cell, TileType tileType, int damage);

        /// <summary>
        /// Tries to place a tile at the given cell coordinates.
        /// </summary>
        /// <returns>Whether the tile was placed successfully.</returns>
        InventoryModification PlaceTile(Vector3Int cell, Placeable placeable);

        bool HasEntity(Vector3Int cell);

        bool HasBlock(Vector3Int cell);
        Placeable GetBlock(Vector3Int cell);
        Placeable GetBlock(Vector3 worldPosition);

        Vector3 CellCenter(Vector3Int cell);
        Bounds CellBoundsWorld(Vector3Int cell);
        Vector3Int WorldToCell(Vector3 worldPosition);
        bool CellIntersects(Vector3Int cell, Bounds other);

        /// <param name="baseCell">The bottom-left cell, NOT center or pivot</param>
        /// <param name="entitySize"></param>
        bool CanAccommodate(Vector3Int baseCell, Vector2Int entitySize);
    }
}
