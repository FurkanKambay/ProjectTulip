using Game.Data.Tiles;
using UnityEngine;

namespace Game.Data.Items
{
    [CreateAssetMenu(fileName = "Pickaxe", menuName = "Items/Pickaxe")]
    public class Pickaxe : ScriptableObject, IUsable
    {
        public float UseTime => useTime;

        public int power = 50;
        [SerializeField] private float useTime = .5f;

        public void Use(Vector3Int cellPosition)
        {
            BlockTile block = World.Instance.Tilemap.GetTile<BlockTile>(cellPosition);
            if (!block) return;

            block.GetHit(power, cellPosition);
        }
    }
}
