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

        public void DamageBlock(Vector3Int cell, int damage)
        {
            if (!tilemap.HasTile(cell)) return;
            if (!TileDamageMap.ContainsKey(cell))
                TileDamageMap.Add(cell, 0);

            BlockTile block = GetBlock(cell);
            int damageTaken = TileDamageMap[cell] += damage;
            int hardness = block.hardness;

            if (damageTaken < hardness)
            {
                BlockHit?.Invoke(cell, block);
                return;
            }

            BlockDestroyed?.Invoke(cell, block);
            tilemap.SetTile(cell, null);
            TileDamageMap.Remove(cell);
        }

        public void PlaceBlock(Vector3Int cell, BlockTile newBlock)
        {
            BlockTile block = GetBlock(cell);
            if (block) return;

            tilemap.SetTile(cell, newBlock);
            TileDamageMap.Remove(cell);
            BlockPlaced?.Invoke(cell, newBlock);
        }

        public int GetTileDamage(Vector3Int cell)
            => TileDamageMap.TryGetValue(cell, out int damage) ? damage : 0;

        public Vector3Int WorldToCell(Vector3 worldPosition) => tilemap.WorldToCell(worldPosition);
        public BlockTile GetBlock(Vector3Int cell) => tilemap.GetTile<BlockTile>(cell);
        public BlockTile GetBlock(Vector3 worldPosition) => GetBlock(WorldToCell(worldPosition));
    }
}
