using Game.Data.Tiles;
using Game.Player;
using UnityEngine;

namespace Game.Items
{
    [CreateAssetMenu(fileName = "Pickaxe", menuName = "Items/Pickaxe")]
    public class Pickaxe : ScriptableObject, IUsable
    {
        public float UseTime => useTime;

        public int power = 50;
        [SerializeField] private float useTime = .5f;

        public void Use(TileModifier modifier, Vector3Int cellPosition)
        {
            BlockTile block = modifier.Tilemap.GetTile<BlockTile>(cellPosition);
            if (!block) return;

            block.GetHit(power, modifier, cellPosition);
        }
    }
}
