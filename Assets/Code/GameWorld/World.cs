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

        private readonly Dictionary<Vector3Int, ITangibleEntity> staticEntities = new();

        private readonly Dictionary<Vector3Int, int> blockDamageMap = new();
        private readonly Dictionary<Vector3Int, int> wallDamageMap = new();
        private readonly Dictionary<Vector3Int, int> curtainDamageMap = new();

        private void OnEnable() => GameState.OnGameStateChange += HandleGameStateChange;
        private void OnDisable() => GameState.OnGameStateChange -= HandleGameStateChange;

        public InventoryModification DamageTile(Vector3Int cell, TileType tileType, int damage)
        {
            if (isReadonly)
                return default;

            // check entities first
            if (HasEntity(cell, out ITangibleEntity tangibleEntity))
            {
                // TODO: pass on damage source
                tangibleEntity.Health.Damage(damage, null);

                if (tangibleEntity.Health.IsDead)
                    staticEntities.Remove(tangibleEntity.Cell);

                Entity entity = tangibleEntity.Entity;
                return !entity.Loot ? default : InventoryModification.ToAdd(entity.Loot.Stack(entity.LootAmount));
            }

            // move on to tiles
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

            if (tilemap.HasTile(cell) || HasEntity(cell))
                return default;

            SetTile(cell, placeable.TileType, placeable);

            GetDamageMap(placeable.TileType).Remove(cell);
            OnPlaceTile?.Invoke(TileModification.FromPlaced(cell, placeable));
            return InventoryModification.ToRemove(placeable.Stack(1));
        }

        public bool TryAddStaticEntity(Vector3Int baseCell, ITangibleEntity entity) =>
            !isReadonly && staticEntities.TryAdd(baseCell, entity);

        public bool HasEntity(Vector3Int cell) =>
            HasEntity(cell, out ITangibleEntity _);

        private bool HasEntity(Vector3Int cell, out ITangibleEntity foundEntity)
        {
            if (staticEntities.TryGetValue(cell, out ITangibleEntity entity))
            {
                foundEntity = entity;
                return true;
            }

            foreach ((Vector3Int position, ITangibleEntity e) in staticEntities)
            {
                var entityRect = new RectInt((Vector2Int)position, e.Entity.Size);

                if (!entityRect.Contains((Vector2Int)cell))
                    continue;

                foundEntity = e;
                return true;
            }

            foundEntity = null;
            return false;
        }

        public void ClearEntities() => staticEntities.Clear();

        public bool HasBlock(Vector3Int cell) => blockTilemap.HasTile(cell);
        public Placeable GetBlock(Vector3Int cell) => GetTile(TileType.Block, cell);
        public Placeable GetBlock(Vector3 worldPosition) => GetBlock(WorldToCell(worldPosition));

        public Vector3 CellCenter(Vector3Int cell) => blockTilemap.GetCellCenterWorld(cell);
        public Bounds CellBoundsWorld(Vector3Int cell) => new(CellCenter(cell), blockTilemap.GetBoundsLocal(cell).size);
        public Vector3Int WorldToCell(Vector3 worldPosition) => blockTilemap.WorldToCell(worldPosition);
        public bool CellIntersects(Vector3Int cell, Bounds other) => CellBoundsWorld(cell).Intersects(other);

        public bool CanAccommodate(Vector3Int baseCell, Vector2Int entitySize)
        {
            if (staticEntities.ContainsKey(baseCell))
                return false;

            var entityRect = new RectInt((Vector2Int)baseCell, entitySize);

            foreach (Vector3Int position in entityRect.allPositionsWithin)
            {
                if (HasBlock(position))
                    return false;
            }

            foreach ((Vector3Int position, ITangibleEntity entity) in staticEntities)
            {
                var rect = new RectInt((Vector2Int)position, entity.Entity.Size);

                if (rect.Overlaps(entityRect))
                    return false;
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
            ClearEntities();
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
