using System;
using System.Collections.Generic;
using Game.Data;
using Game.Data.Tiles;
using Game.Helpers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class World : Singleton<World>
    {
        [SerializeField] Tilemap tilemap;

        public event Action<Vector3Int, WorldTile> OnPlaceTile;
        public event Action<Vector3Int, WorldTile> OnHitTile;
        public event Action<Vector3Int, WorldTile> OnDestroyTile;

        private readonly Dictionary<Vector3Int, int> tileDamageMap = new();

        /// <summary>
        /// Damages a tile at the given cell coordinates.
        /// </summary>
        /// <returns>Whether the tile was destroyed.</returns>
        public InventoryModification DamageTile(Vector3Int cell, int damage)
        {
            if (!tilemap.HasTile(cell))
                return InventoryModification.Empty;

            tileDamageMap.TryAdd(cell, 0);

            WorldTile tile = GetTile(cell);
            int damageTaken = tileDamageMap[cell] += damage;
            int hardness = tile.hardness;

            if (damageTaken < hardness)
            {
                OnHitTile?.Invoke(cell, tile);
                return InventoryModification.Empty;
            }

            tilemap.SetTile(cell, null);
            tileDamageMap.Remove(cell);
            OnDestroyTile?.Invoke(cell, tile);
            return new InventoryModification(toAdd: new ItemStack(item: tile));
        }

        /// <summary>
        /// Tries to place a tile at the given cell coordinates.
        /// </summary>
        /// <returns>Whether the tile was placed successfully.</returns>
        public InventoryModification PlaceTile(Vector3Int cell, WorldTile tile)
        {
            if (tilemap.HasTile(cell))
                return InventoryModification.Empty;

            tilemap.SetTile(cell, tile);
            tileDamageMap.Remove(cell);
            OnPlaceTile?.Invoke(cell, tile);
            return new InventoryModification(toRemove: new ItemStack(item: tile));
        }

        public int GetTileDamage(Vector3Int cell)
            => tileDamageMap.GetValueOrDefault(cell, 0);

        public bool CellIntersects(Vector3Int cell, Bounds other)
            => CellBoundsWorld(cell).Intersects(other);

        public Vector3Int WorldToCell(Vector3 worldPosition) => tilemap.WorldToCell(worldPosition);
        public Vector3 CellCenter(Vector3Int cell) => tilemap.GetCellCenterWorld(cell);
        public Bounds CellBoundsWorld(Vector3Int cell) => new(CellCenter(cell), tilemap.GetBoundsLocal(cell).size);

        public bool HasTile(Vector3Int cell) => tilemap.HasTile(cell);
        public WorldTile GetTile(Vector3Int cell) => tilemap.GetTile<WorldTile>(cell);
        public WorldTile GetTile(Vector3 worldPosition) => GetTile(WorldToCell(worldPosition));
    }
}
