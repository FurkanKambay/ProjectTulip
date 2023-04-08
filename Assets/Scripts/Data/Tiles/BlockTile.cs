using Game.Data.Interfaces;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Data.Tiles
{
    [CreateAssetMenu(fileName = "Block", menuName = "Items/Block")]
    public class BlockTile : RuleTile<BlockTile.Neighbor>, ITool
    {
        public float Cooldown => .25f;

        public Sprite Icon => m_DefaultSprite;
        public float IconScale => iconScale;
        public int MaxAmount => maxAmount;

        public Color color = Color.white;
        [SerializeField] float iconScale = 1f;
        [SerializeField, Min(1)] private int maxAmount = 999;

        [Header("Block Data")]
        public int hardness = 50;

        [Header("Sounds")]
        public AudioClip hitSound;
        public AudioClip placeSound;

        public bool CanUseOnBlock(BlockTile block) => block != this;

        public override bool RuleMatch(int neighbor, TileBase tile) => neighbor switch
        {
            Neighbor.Null => tile == null,
            Neighbor.NotNull => tile != null,
            _ => base.RuleMatch(neighbor, tile)
        };

        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
        {
            tileData.color = this.color;
            base.GetTileData(location, tilemap, ref tileData);
        }

        public class Neighbor : RuleTile.TilingRuleOutput.Neighbor
        {
            public const int Null = 3;
            public const int NotNull = 4;
        }
    }
}
