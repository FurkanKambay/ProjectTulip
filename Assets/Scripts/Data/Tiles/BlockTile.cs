using Game.Data.Items;
using UnityEngine;

namespace Game.Data.Tiles
{
    [CreateAssetMenu(fileName = "Block", menuName = "Items/Block")]
    public class BlockTile : CustomTile, IUsable
    {
        [Header("Data")]
        public int hardness = 50;

        public float UseTime => .25f;

        public void Use(Vector3Int cellPosition)
            => World.Instance.Tilemap.SetTile(cellPosition, this);

        public void GetHit(int damage, Vector3Int cell)
        {
            if (!World.Instance.TileDamageMap.ContainsKey(cell))
                World.Instance.TileDamageMap.Add(cell, 0);

            int damageTaken = World.Instance.TileDamageMap[cell] += damage;
            if (damageTaken >= hardness)
                World.Instance.Tilemap.SetTile(cell, null);
        }
    }
}
