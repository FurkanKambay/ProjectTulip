using Tulip.Data.Items;
using UnityEngine;

// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Tulip.Data
{
    public interface IWorld
    {
        public delegate void PlaceableEvent(TileModification modification);

        event IWorldProvider.ProvideWorldEvent OnRefresh;
        event PlaceableEvent OnPlaceTile;
        event PlaceableEvent OnHitTile;
        event PlaceableEvent OnDestroyTile;

        WorldData WorldData { get; }
        bool IsReadonly { get; }

        /// <summary>
        /// Damages a tile of the given type at the given cell coordinates.
        /// </summary>
        /// <returns>Whether the tile was destroyed.</returns>
        InventoryModification DamageTile(Vector2Int cell, TileType tileType, int damage);

        /// <summary>
        /// Tries to place a tile at the given cell coordinates.
        /// </summary>
        /// <returns>Whether the tile was placed successfully.</returns>
        InventoryModification PlaceTile(Vector2Int cell, PlaceableData placeableData);

        bool HasTile(Vector2Int cell, TileType tileType);
        PlaceableData GetTile(Vector2Int cell, TileType tileType);
        PlaceableData GetTileAtWorld(Vector3 worldPosition, TileType tileType);

        Vector3 CellCenter(Vector2Int cell);
        Vector2Int WorldToCell(Vector3 worldPosition);

        Bounds CellBoundsWorld(Vector2Int cell);
        bool DoesCellIntersect(Vector2Int cell, Bounds other);
        bool IsCellEntityFree(Vector2Int cell);

        /// <param name="baseCell">The bottom-left cell, NOT center or pivot</param>
        /// <param name="entitySize"></param>
        bool CanAccommodate(Vector2Int baseCell, Vector2Int entitySize);
    }
}
