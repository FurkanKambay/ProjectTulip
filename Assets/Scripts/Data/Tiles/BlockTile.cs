using System.Collections.Generic;
using Game.Data.Items;
using Game.Player;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Data.Tiles
{
    [CreateAssetMenu(fileName = "Block", menuName = "Items/Block")]
    public class BlockTile : CustomTile, IUsable
    {
        public float UseTime => .25f;

        [Header("Data")]
        public int hardness = 50;
        public AudioClip hitSound;

        private static Dictionary<Vector3Int, int> DamageMap => World.Instance.TileDamageMap;
        private static Tilemap Tilemap => World.Instance.Tilemap;

        public void Use(Vector3Int cellPosition)
        {
            BlockTile block = Tilemap.GetTile<BlockTile>(cellPosition);
            if (block && block != this)
            {
                Inventory.Instance.ActivePickaxe.Use(cellPosition);
                return;
            }

            Tilemap.SetTile(cellPosition, this);
        }

        public void GetHit(int damage, Vector3Int cell)
        {
            if (!DamageMap.ContainsKey(cell))
                DamageMap.Add(cell, 0);

            AudioSource.PlayClipAtPoint(hitSound, Tilemap.CellToWorld(cell));

            int damageTaken = DamageMap[cell] += damage;
            if (damageTaken >= hardness)
                Tilemap.SetTile(cell, null);
        }
    }
}
