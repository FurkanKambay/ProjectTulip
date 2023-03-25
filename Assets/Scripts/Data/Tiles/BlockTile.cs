using Game.Data.Items;
using UnityEngine;

namespace Game.Data.Tiles
{
    [CreateAssetMenu(fileName = "Block", menuName = "Items/Block")]
    public class BlockTile : CustomTile, IUsable
    {
        public float UseTime => .25f;
        public Sprite Icon => sprite;

        [Header("Data")]
        public int hardness = 50;

        [Header("Sounds")]
        public AudioClip hitSound;
        public AudioClip placeSound;

        public void Use(Vector3Int cell, Pickaxe pickaxe)
        {
            BlockTile block = World.Instance.GetBlock(cell);

            if (block && block != this)
                pickaxe.Use(cell);
            else
                World.Instance.PlaceBlock(cell, this);
        }
    }
}
