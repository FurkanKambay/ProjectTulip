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
        public Tilemap Tilemap => tilemap;

        [SerializeField] Tilemap tilemap;

        private Dictionary<Vector3Int, int> TileDamageMap { get; } = new();

        public event Action<Vector3Int, BlockTile> OnPlaceBlock;
        public event Action<Vector3Int, BlockTile> OnHitBlock;
        public event Action<Vector3Int, BlockTile> OnDestroyBlock;

        /// <summary>
        /// Damages a block at the given cell coordinates.
        /// </summary>
        /// <returns>Whether the block was destroyed.</returns>
        public InventoryModification DamageBlock(Vector3Int cell, int damage)
        {
            if (!Tilemap.HasTile(cell))
                return InventoryModification.Empty;

            TileDamageMap.TryAdd(cell, 0);

            BlockTile block = GetBlock(cell);
            int damageTaken = TileDamageMap[cell] += damage;
            int hardness = block.hardness;

            if (damageTaken < hardness)
            {
                OnHitBlock?.Invoke(cell, block);
                return InventoryModification.Empty;
            }

            Tilemap.SetTile(cell, null);
            TileDamageMap.Remove(cell);
            OnDestroyBlock?.Invoke(cell, block);
            return new InventoryModification(toAdd: new ItemStack(item: block));
        }

        /// <summary>
        /// Tries to place a block at the given cell coordinates.
        /// </summary>
        /// <returns>Whether the block was placed successfully.</returns>
        public InventoryModification PlaceBlock(Vector3Int cell, BlockTile newBlock)
        {
            if (Tilemap.HasTile(cell))
                return InventoryModification.Empty;

            Tilemap.SetTile(cell, newBlock);
            TileDamageMap.Remove(cell);
            OnPlaceBlock?.Invoke(cell, newBlock);
            return new InventoryModification(toRemove: new ItemStack(item: newBlock));
        }

        public int GetTileDamage(Vector3Int cell)
            => TileDamageMap.GetValueOrDefault(cell, 0);

        public bool CellIntersects(Vector3Int cell, Bounds other)
            => CellBoundsWorld(cell).Intersects(other);

        public Vector3Int WorldToCell(Vector3 worldPosition) => Tilemap.WorldToCell(worldPosition);
        public Vector3 CellCenter(Vector3Int cell) => Tilemap.GetCellCenterWorld(cell);
        public Bounds CellBoundsWorld(Vector3Int cell) => new(CellCenter(cell), Tilemap.GetBoundsLocal(cell).size);

        public bool HasBlock(Vector3Int cell) => Tilemap.HasTile(cell);
        public BlockTile GetBlock(Vector3Int cell) => Tilemap.GetTile<BlockTile>(cell);
        public BlockTile GetBlock(Vector3 worldPosition) => GetBlock(WorldToCell(worldPosition));
    }
}
