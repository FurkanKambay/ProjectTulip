using System;
using UnityEngine.Assertions;
using System.Collections.Generic;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Items;
using Tulip.Data.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tulip.GameWorld
{
    public class World : Singleton<World>, IWorld
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

            WorldTile worldTile = GetTile(cell).WorldTile;
            int damageTaken = tileDamageMap[cell] += damage;
            int hardness = worldTile.hardness;

            if (damageTaken < hardness)
            {
                OnHitTile?.Invoke(cell, worldTile);
                return InventoryModification.Empty;
            }

            tilemap.SetTile(cell, null);
            tileDamageMap.Remove(cell);
            OnDestroyTile?.Invoke(cell, worldTile);
            return new InventoryModification(toAdd: new ItemStack(item: worldTile));
        }

        /// <summary>
        /// Tries to place a tile at the given cell coordinates.
        /// </summary>
        /// <returns>Whether the tile was placed successfully.</returns>
        public InventoryModification PlaceTile(Vector3Int cell, WorldTile worldTile)
        {
            if (tilemap.HasTile(cell))
                return InventoryModification.Empty;

            // var tileChangeData = new TileChangeData(cell, worldTile.Tile, worldTile.Color, Matrix4x4.zero);
            // tilemap.SetTile(tileChangeData, ignoreLockFlags: false);
            tilemap.SetTile(cell, worldTile.RuleTile);
            tilemap.SetColor(cell, worldTile.color);
            // BUG: color doesn't work

            tileDamageMap.Remove(cell);
            OnPlaceTile?.Invoke(cell, worldTile);
            return new InventoryModification(toRemove: new ItemStack(item: worldTile));
        }

        public bool CanAccommodate(Vector3Int cell, Vector2Int entitySize)
        {
            Assert.IsTrue(entitySize.x > 0, "entitySize.x > 0");
            Assert.IsTrue(entitySize.y > 0, "entitySize.y > 0");

            // NOTE: temporary restriction
            Assert.IsTrue(entitySize.x == 1, "entitySize.x == 1");

            Vector3Int cellToCheck = cell;

            var floor = new Vector3Int(cellToCheck.x, cellToCheck.y - 1);
            if (!HasTile(floor) || !GetTile(floor).WorldTile.IsSafe)
                return false;

            for (int y = 0; y < entitySize.y; y++)
            {
                if (HasTile(cellToCheck))
                    return false;

                cellToCheck.y++;
            }

            return true;
        }

        public int GetTileDamage(Vector3Int cell)
            => tileDamageMap.GetValueOrDefault(cell, 0);

        public bool CellIntersects(Vector3Int cell, Bounds other)
            => CellBoundsWorld(cell).Intersects(other);

        public Vector3Int WorldToCell(Vector3 worldPosition) => tilemap.WorldToCell(worldPosition);
        public Vector3 CellCenter(Vector3Int cell) => tilemap.GetCellCenterWorld(cell);
        public Bounds CellBoundsWorld(Vector3Int cell) => new(CellCenter(cell), tilemap.GetBoundsLocal(cell).size);

        public bool HasTile(Vector3Int cell) => tilemap.HasTile(cell);
        public CustomRuleTile GetTile(Vector3Int cell) => tilemap.GetTile<CustomRuleTile>(cell);
        public CustomRuleTile GetTile(Vector3 worldPosition) => GetTile(WorldToCell(worldPosition));
    }
}
