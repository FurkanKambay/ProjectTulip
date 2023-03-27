using System;
using System.Collections.Generic;
using Game.Data.Tiles;
using Game.Helpers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class World : Singleton<World>
    {
        [SerializeField] private Tilemap tilemap;

        private Dictionary<Vector3Int, int> TileDamageMap { get; } = new();

        public event Action<Vector3Int, BlockTile> BlockPlaced;
        public event Action<Vector3Int, BlockTile> BlockHit;
        public event Action<Vector3Int, BlockTile> BlockDestroyed;

        /// <summary>
        /// Damages a block at the given cell.
        /// </summary>
        /// <param name="invokeDestroyEvent">Whether to invoke the BlockDestroyed event.</param>
        /// <returns>Whether the block is destroyed.</returns>
        public bool DamageBlock(Vector3Int cell, int damage, bool invokeDestroyEvent = true)
        {
            if (!tilemap.HasTile(cell)) return false;
            if (!TileDamageMap.ContainsKey(cell))
                TileDamageMap.Add(cell, 0);

            BlockTile block = GetBlock(cell);
            int damageTaken = TileDamageMap[cell] += damage;
            int hardness = block.hardness;

            if (damageTaken < hardness)
            {
                BlockHit?.Invoke(cell, block);
                return false;
            }

            if (invokeDestroyEvent)
                BlockDestroyed?.Invoke(cell, block);

            tilemap.SetTile(cell, null);
            TileDamageMap.Remove(cell);
            return true;
        }

        /// <summary>
        /// Places a block at the given cell.
        /// </summary>
        /// <param name="damage">Damage amount if cell is not empty.</param>
        /// <returns>Whether the block was placed successfully.</returns>
        public bool PlaceBlock(Vector3Int cell, BlockTile newBlock, int damage = 0)
        {
            BlockTile block = GetBlock(cell);
            if (block == newBlock) return false;

            if (block && !DamageBlock(cell, damage, invokeDestroyEvent: false))
                return false;

            tilemap.SetTile(cell, newBlock);
            TileDamageMap.Remove(cell);
            BlockPlaced?.Invoke(cell, newBlock);
            return true;
        }

        public int GetTileDamage(Vector3Int cell)
            => TileDamageMap.TryGetValue(cell, out int damage) ? damage : 0;

        public Vector3Int WorldToCell(Vector3 worldPosition) => tilemap.WorldToCell(worldPosition);
        public Vector3 CellCenter(Vector3Int cell) => tilemap.GetCellCenterWorld(cell);
        public Bounds CellBoundsWorld(Vector3Int cell) => new(CellCenter(cell), tilemap.GetBoundsLocal(cell).size);

        public bool HasBlock(Vector3Int cell) => tilemap.HasTile(cell);
        public BlockTile GetBlock(Vector3Int cell) => tilemap.GetTile<BlockTile>(cell);
        public BlockTile GetBlock(Vector3 worldPosition) => GetBlock(WorldToCell(worldPosition));
    }
}
