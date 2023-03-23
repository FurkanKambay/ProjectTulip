using Game.Items;
using Game.Player;
using UnityEngine;

namespace Game.Data.Tiles
{
    [CreateAssetMenu(fileName = "Block", menuName = "Items/Block")]
    public class BlockTile : CustomTile, IUsable
    {
        [Header("Data")]
        public int hardness = 50;

        public float UseTime => .25f;

        public void Use(TileModifier modifier, Vector3Int cellPosition)
            => modifier.Tilemap.SetTile(cellPosition, this);

        public void GetHit(int damage, TileModifier modifier, Vector3Int cell)
        {
            if (!modifier.TileDamageMap.ContainsKey(cell))
                modifier.TileDamageMap.Add(cell, 0);

            int damageTaken = modifier.TileDamageMap[cell] += damage;
            if (damageTaken >= hardness)
                modifier.Tilemap.SetTile(cell, null);
        }
    }
}
