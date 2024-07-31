using System;
using System.Collections.Generic;
using SaintsField.Playa;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Items;
using Tulip.Data.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tulip.GameWorld
{
    public class World : MonoBehaviour, IWorld
    {
        [Header("References")]
        [SerializeField] Tilemap wallTilemap;
        [SerializeField] Tilemap blockTilemap;
        [SerializeField] Tilemap curtainTilemap;

        [Header("Config")]
        public bool isReadonly;

        public event IWorld.PlaceableEvent OnPlaceTile;
        public event IWorld.PlaceableEvent OnHitTile;
        public event IWorld.PlaceableEvent OnDestroyTile;

        public Vector2Int Size { get; internal set; }

        private readonly Dictionary<Vector3Int, int> blockDamageMap = new();
        private readonly Dictionary<Vector3Int, int> wallDamageMap = new();
        private readonly Dictionary<Vector3Int, int> curtainDamageMap = new();

        private void OnEnable() => GameState.OnGameStateChange += HandleGameStateChange;
        private void OnDisable() => GameState.OnGameStateChange -= HandleGameStateChange;

        public InventoryModification DamageTile(Vector3Int cell, TileType tileType, int damage)
        {
            if (isReadonly)
                return default;

            Tilemap tilemap = GetTilemap(tileType);
            Placeable placeable = GetTile(tileType, cell);

            if (!tilemap.HasTile(cell) || placeable.IsUnbreakable)
                return default;

            Dictionary<Vector3Int, int> damageMap = GetDamageMap(tileType);
            damageMap.TryAdd(cell, 0);

            int damageTaken = damageMap[cell] += damage;
            int hardness = placeable.Hardness;

            if (damageTaken < hardness)
            {
                OnHitTile?.Invoke(TileModification.FromDamaged(cell, placeable));
                return default;
            }

            tilemap.SetTile(cell, null);
            damageMap.Remove(cell);
            OnDestroyTile?.Invoke(TileModification.FromDestroyed(cell, placeable));

            Item item = placeable.Ore ? placeable.Ore : placeable;
            return InventoryModification.ToAdd(item.Stack(1));
        }

        public InventoryModification PlaceTile(Vector3Int cell, Placeable placeable)
        {
            if (isReadonly)
                return default;

            Tilemap tilemap = GetTilemap(placeable.TileType);

            if (tilemap.HasTile(cell))
                return default;

            SetTile(cell, placeable.TileType, placeable);

            GetDamageMap(placeable.TileType).Remove(cell);
            OnPlaceTile?.Invoke(TileModification.FromPlaced(cell, placeable));
            return InventoryModification.ToRemove(placeable.Stack(1));
        }

        public bool HasBlock(Vector3Int cell) => blockTilemap.HasTile(cell);
        public Placeable GetBlock(Vector3Int cell) => GetTile(TileType.Block, cell);
        public Placeable GetBlock(Vector3 worldPosition) => GetBlock(WorldToCell(worldPosition));

        public Vector3 CellCenter(Vector3Int cell) => blockTilemap.GetCellCenterWorld(cell);
        public Bounds CellBoundsWorld(Vector3Int cell) => new(CellCenter(cell), blockTilemap.GetBoundsLocal(cell).size);
        public Vector3Int WorldToCell(Vector3 worldPosition) => blockTilemap.WorldToCell(worldPosition);
        public bool CellIntersects(Vector3Int cell, Bounds other) => CellBoundsWorld(cell).Intersects(other);

        public bool CanAccommodate(Vector3Int cell, Vector2Int entitySize)
        {
            Vector3Int cellToCheck = cell;

            for (int y = 0; y < entitySize.y; y++, cellToCheck.y++)
            {
                for (int x = 0; x < entitySize.x; x++, cellToCheck.x++)
                {
                    if (HasBlock(cell))
                        return false;
                }
            }

            return true;
        }

        public int GetTileDamage(Vector3Int cell, TileType tileType) =>
            GetDamageMap(tileType).GetValueOrDefault(cell, 0);

        internal void SetTiles(TileType tileType, TileChangeData[] tileChangeData) =>
            GetTilemap(tileType).SetTiles(tileChangeData, ignoreLockFlags: true);

        [Button]
        internal void ResetTilemaps()
        {
            ResetTilemap(wallTilemap);
            ResetTilemap(blockTilemap);
            ResetTilemap(curtainTilemap);
        }

        private Placeable GetTile(TileType tileType, Vector3Int cell) =>
            GetTilemap(tileType).GetTile<CustomRuleTile>(cell)?.Placeable;

        private void SetTile(Vector3Int cell, TileType tileType, Placeable placeable)
        {
            Tilemap tilemap = GetTilemap(tileType);

            if (!placeable)
            {
                tilemap.SetTile(cell, null);
                return;
            }

            tilemap.SetTile(cell, placeable.RuleTile);
            tilemap.SetColor(cell, placeable.Color);
            // BUG: color doesn't work
        }

        private void ResetTilemap(Tilemap tilemap)
        {
            tilemap.ClearAllTiles();
            tilemap.size = new Vector3Int(Size.x, Size.y, 1);
            tilemap.CompressBounds();
            tilemap.transform.position = new Vector3(-Size.x / 2f, -Size.y, 0);
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

        private void HandleGameStateChange(GameState oldState, GameState newState) =>
            isReadonly = newState == GameState.MainMenu;
    }
}
