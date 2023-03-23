using System.Collections.Generic;
using Game.Data.Items;
using UnityEngine;

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

        public void Use(Vector3Int cellPosition)
            => World.Instance.Tilemap.SetTile(cellPosition, this);

        public void GetHit(int damage, Vector3Int cell)
        {
            if (!DamageMap.ContainsKey(cell))
                DamageMap.Add(cell, 0);

            AudioSource.PlayClipAtPoint(hitSound, World.Instance.Tilemap.CellToWorld(cell));

            int damageTaken = DamageMap[cell] += damage;
            if (damageTaken >= hardness)
                World.Instance.Tilemap.SetTile(cell, null);
        }
    }
}
