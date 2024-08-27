using System;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.GameWorld
{
    public class World : MonoBehaviour, IWorld
    {
        [Header("References")]
        [SerializeField] SaintsInterface<Component, IWorldProvider> worldProvider;
        [SerializeField] WorldVisual worldVisual;

        [Header("Config")]
        [SerializeField] bool isReadonly;

        public event IWorldProvider.ProvideWorldEvent OnRefresh;
        public event IWorld.PlaceableEvent OnPlaceTile;
        public event IWorld.PlaceableEvent OnHitTile;
        public event IWorld.PlaceableEvent OnDestroyTile;

        public Vector2Int Dimensions => WorldData.Dimensions;
        public bool IsReadonly => isReadonly;

        internal WorldData WorldData { get; private set; }

        private readonly Dictionary<Vector2Int, ITangibleEntity> staticEntities = new();

        private readonly Dictionary<Vector2Int, int> wallDamageMap = new();
        private readonly Dictionary<Vector2Int, int> blockDamageMap = new();
        private readonly Dictionary<Vector2Int, int> curtainDamageMap = new();

        private void OnEnable()
        {
            GameManager.OnGameStateChange += HandleGameStateChange;
            worldProvider.I.OnProvideWorld += HandleWorldProvided;
        }

        private void OnDisable()
        {
            GameManager.OnGameStateChange -= HandleGameStateChange;
            worldProvider.I.OnProvideWorld -= HandleWorldProvided;
        }

        public InventoryModification DamageTile(Vector2Int cell, TileType tileType, int damage)
        {
            if (isReadonly)
                return default;

            Dictionary<Vector2Int, PlaceableData> tiles = GetTiles(tileType);
            PlaceableData placeableData = tiles[cell];

            if (!tiles.ContainsKey(cell))
                return default;

            if (placeableData.IsUnbreakable)
            {
                // TODO: feedback for 'unbreakable'
                // change return type to account for this
                return default;
            }

            Dictionary<Vector2Int, int> damageMap = GetDamageMap(tileType);
            damageMap.TryAdd(cell, 0);
            damageMap[cell] += damage;

            if (damageMap[cell] < placeableData.Hardness)
            {
                // the tile was not destroyed
                OnHitTile?.Invoke(TileModification.FromDamaged(cell, placeableData));
                return default;
            }

            tiles.Remove(cell);
            damageMap.Remove(cell);

            OnDestroyTile?.Invoke(TileModification.FromDestroyed(cell, placeableData));

            ItemData loot = placeableData.OreData ? placeableData.OreData : placeableData;
            return InventoryModification.ToAdd(loot.Stack(1));
        }

        public InventoryModification PlaceTile(Vector2Int cell, PlaceableData placeableData)
        {
            if (isReadonly)
                return default;

            Dictionary<Vector2Int, PlaceableData> tiles = GetTiles(placeableData.TileType);

            if (!tiles.TryAdd(cell, placeableData))
                return default;

            GetDamageMap(placeableData.TileType).Remove(cell);
            OnPlaceTile?.Invoke(TileModification.FromPlaced(cell, placeableData));

            return InventoryModification.ToRemove(placeableData.Stack(1));
        }

        public bool TryAddStaticEntity(Vector2Int baseCell, ITangibleEntity entity) =>
            !isReadonly && staticEntities.TryAdd(baseCell, entity);

        public void ClearEntities() => staticEntities.Clear();

#region Cell Helpers

        public bool HasWall(Vector2Int cell) => WorldData.Walls.ContainsKey(cell);
        public bool HasBlock(Vector2Int cell) => WorldData.Blocks.ContainsKey(cell);
        public bool HasCurtain(Vector2Int cell) => WorldData.Curtains.ContainsKey(cell);

        public PlaceableData GetWall(Vector2Int cell) => WorldData.Walls.GetValueOrDefault(cell);
        public PlaceableData GetBlock(Vector2Int cell) => WorldData.Blocks.GetValueOrDefault(cell);
        public PlaceableData GetCurtain(Vector2Int cell) => WorldData.Curtains.GetValueOrDefault(cell);

        public PlaceableData GetWallAtWorld(Vector3 worldPosition) => GetWall(WorldToCell(worldPosition));
        public PlaceableData GetBlockAtWorld(Vector3 worldPosition) => GetBlock(WorldToCell(worldPosition));
        public PlaceableData GetCurtainAtWorld(Vector3 worldPosition) => GetCurtain(WorldToCell(worldPosition));

        public Vector3 CellCenter(Vector2Int cell) => worldVisual.GetCellCenterWorld(cell);
        public Vector2Int WorldToCell(Vector3 worldPosition) => worldVisual.WorldToCell(worldPosition);

        public Bounds CellBoundsWorld(Vector2Int cell) => worldVisual.CellBoundsWorld(cell);
        public bool DoesCellIntersect(Vector2Int cell, Bounds other) => CellBoundsWorld(cell).Intersects(other);

#endregion

        public bool CanAccommodate(Vector2Int baseCell, Vector2Int entitySize)
        {
            if (staticEntities.ContainsKey(baseCell))
                return false;

            var entityRect = new RectInt(baseCell, entitySize);

            foreach (Vector2Int position in entityRect.allPositionsWithin)
            {
                if (HasBlock(position))
                    return false;
            }

            return staticEntities.Values.All(entity => !entity.Rect.Overlaps(entityRect));
        }

        public int GetTileDamage(Vector2Int cell, TileType tileType) =>
            GetDamageMap(tileType).GetValueOrDefault(cell, 0);

        private void HandleWorldProvided(WorldData worldData)
        {
            WorldData = worldData;
            OnRefresh?.Invoke(worldData);
        }

        private void HandleGameStateChange(GameState oldState, GameState newState) =>
            isReadonly = newState == GameState.MainMenu;

        private Dictionary<Vector2Int, PlaceableData> GetTiles(TileType tileType) => tileType switch
        {
            TileType.Wall => WorldData.Walls,
            TileType.Block => WorldData.Blocks,
            TileType.Curtain => WorldData.Curtains,
            _ => throw new ArgumentOutOfRangeException(nameof(tileType))
        };

        private Dictionary<Vector2Int, int> GetDamageMap(TileType tileType) => tileType switch
        {
            TileType.Wall => wallDamageMap,
            TileType.Block => blockDamageMap,
            TileType.Curtain => curtainDamageMap,
            _ => throw new ArgumentOutOfRangeException(nameof(tileType))
        };
    }
}
