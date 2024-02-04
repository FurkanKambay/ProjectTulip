using Game.Data.Interfaces;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Data.Tiles
{
    [CreateAssetMenu(fileName = "Tile", menuName = "Items/Tile")]
    public class WorldTile : RuleTile<WorldTile.Neighbor>, ITool
    {
        public float Cooldown => 0.25f;
        public float ChargeTime => 0f;
        public float SwingTime => 0f;
        public bool IsSafe => true;

        public ItemType Type => type;
        public Sprite Icon => m_DefaultSprite;
        public float IconScale => iconScale;
        public string Name => name;
        public string Description => description;
        public int MaxAmount => maxAmount;

        [Header("Tile Data")]
        public Color color = Color.white;

        [Header("Item Data")]
        [SerializeField] ItemType type;
        [SerializeField] float iconScale = 1f;
        [SerializeField] new string name;
        [SerializeField, Multiline] string description;
        [SerializeField, Min(1)] int maxAmount = 999;

        [Header("World Tile Data")]
        [Min(1)] public int hardness = 50;

        [Header("Sounds")]
        public AudioClip hitSound;
        public AudioClip destroySound;
        public AudioClip placeSound;

        public bool IsUsableOnTile(WorldTile tile) => tile is null;

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

        // ReSharper disable once ClassNeverInstantiated.Global
        public class Neighbor : RuleTile.TilingRuleOutput.Neighbor
        {
            public const int Null = 3;
            public const int NotNull = 4;
        }
    }
}
