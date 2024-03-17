using System;
using UnityEngine.Assertions;
using System.Collections.Generic;
using Tulip.Data;
using Tulip.Data.Items;
using Tulip.Data.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tulip.GameWorld
{
    public class World : MonoBehaviour, IWorld
    {
        public Vector2Int Size { get; internal set; }

        [SerializeField] Tilemap wallTilemap;
        [SerializeField] Tilemap blockTilemap;
        [SerializeField] Tilemap curtainTilemap;

        public event Action<TileModification> OnPlaceTile;
        public event Action<TileModification> OnHitTile;
        public event Action<TileModification> OnDestroyTile;

        private readonly Dictionary<Vector3Int, int> blockDamageMap = new();
        private readonly Dictionary<Vector3Int, int> wallDamageMap = new();
        private readonly Dictionary<Vector3Int, int> curtainDamageMap = new();

        public InventoryModification DamageTile(Vector3Int cell, TileType tileType, int damage)
        {
            Tilemap tilemap = GetTilemap(tileType);
            Dictionary<Vector3Int, int> damageMap = GetDamageMap(tileType);

            if (!tilemap.HasTile(cell))
                return InventoryModification.Empty;

            damageMap.TryAdd(cell, 0);

            WorldTile worldTile = GetTile(cell);
            int damageTaken = damageMap[cell] += damage;
            int hardness = worldTile.hardness;

            if (damageTaken < hardness)
            {
                OnHitTile?.Invoke(TileModification.FromDamaged(cell, worldTile));
                return InventoryModification.Empty;
            }

            tilemap.SetTile(cell, null);
            damageMap.Remove(cell);
            OnDestroyTile?.Invoke(TileModification.FromDestroyed(cell, worldTile));
            return new InventoryModification(toAdd: new ItemStack(item: worldTile));
        }

        public InventoryModification PlaceTile(Vector3Int cell, WorldTile worldTile)
        {
            Tilemap tilemap = GetTilemap(worldTile.TileType);

            if (tilemap.HasTile(cell))
                return InventoryModification.Empty;

            SetTile(cell, worldTile.TileType, worldTile);

            GetDamageMap(worldTile.TileType).Remove(cell);
            OnPlaceTile?.Invoke(TileModification.FromPlaced(cell, worldTile));
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
            if (!HasTile(floor) || !GetTile(floor).IsSafe)
                return false;

            for (int y = 0; y < entitySize.y; y++)
            {
                if (HasTile(cellToCheck))
                    return false;

                cellToCheck.y++;
            }

            return true;
        }

        public int GetTileDamage(Vector3Int cell, TileType tileType)
            => GetDamageMap(tileType).GetValueOrDefault(cell, 0);

        public bool CellIntersects(Vector3Int cell, Bounds other)
            => CellBoundsWorld(cell).Intersects(other);

        public Vector3Int WorldToCell(Vector3 worldPosition) => blockTilemap.WorldToCell(worldPosition);
        public Vector3 CellCenter(Vector3Int cell) => blockTilemap.GetCellCenterWorld(cell);
        public Bounds CellBoundsWorld(Vector3Int cell) => new(CellCenter(cell), blockTilemap.GetBoundsLocal(cell).size);

        public bool HasTile(Vector3Int cell) => blockTilemap.HasTile(cell);
        public WorldTile GetTile(Vector3Int cell) => blockTilemap.GetTile<CustomRuleTile>(cell)?.WorldTile;
        public WorldTile GetTile(Vector3 worldPosition) => GetTile(WorldToCell(worldPosition));

        internal void SetTile(Vector3Int cell, TileType tileType, WorldTile worldTile)
        {
            Tilemap tilemap = GetTilemap(tileType);

            // var tileChangeData = new TileChangeData(cell, worldTile.Tile, worldTile.Color, Matrix4x4.zero);
            // tilemap.SetTile(tileChangeData, ignoreLockFlags: false);

            if (worldTile)
            {
                tilemap.SetTile(cell, worldTile.RuleTile);
                tilemap.SetColor(cell, worldTile.color);
                // BUG: color doesn't work
            }
            else
            {
                tilemap.SetTile(cell, null);
            }
        }

        [ContextMenu("Reset Tilemaps")]
        internal void ResetTilemaps()
        {
            wallTilemap.ClearAllTiles();
            blockTilemap.ClearAllTiles();
            curtainTilemap.ClearAllTiles();
            ResetTilemapTransforms();
        }

        private void ResetTilemapTransforms()
        {
            wallTilemap.size = new Vector3Int(Size.x, Size.y, 1);
            blockTilemap.size = new Vector3Int(Size.x, Size.y, 1);
            curtainTilemap.size = new Vector3Int(Size.x, Size.y, 1);

            wallTilemap.CompressBounds();
            blockTilemap.CompressBounds();
            curtainTilemap.CompressBounds();

            blockTilemap.transform.position = new Vector3(-Size.x / 2f, -Size.y, 0);
            wallTilemap.transform.position = new Vector3(-Size.x / 2f, -Size.y, 0);
            curtainTilemap.transform.position = new Vector3(-Size.x / 2f, -Size.y, 0);
        }

        private Tilemap GetTilemap(TileType tileType) => tileType switch
        {
            TileType.Wall => wallTilemap,
            TileType.Block => blockTilemap,
            TileType.Curtain => curtainTilemap,
            _ => throw new ArgumentOutOfRangeException()
        };

        private Dictionary<Vector3Int, int> GetDamageMap(TileType tileType) => tileType switch
        {
            TileType.Wall => wallDamageMap,
            TileType.Block => blockDamageMap,
            TileType.Curtain => curtainDamageMap,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
