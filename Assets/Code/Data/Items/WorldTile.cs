using SaintsField.Playa;
using Tulip.Data.Tiles;
using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/World Tile")]
    public class WorldTile : WorldToolBase
    {
        public override Sprite Icon => ruleTile.m_DefaultSprite;

        public Color Color => color;
        public CustomRuleTile RuleTile => ruleTile;
        public TileType TileType => tileType;

        public bool IsUnsafe => isUnsafe;
        public bool IsUnbreakable => isUnbreakable;
        public int Hardness => hardness;
        public Ore Ore => ore;

        public AudioClip PlaceSound => placeSound;
        public AudioClip HitSound => hitSound;
        public AudioClip DestroySound => destroySound;

        [Header("World Tile Data")]
        [SerializeField] protected Color color;
        [SerializeField] protected CustomRuleTile ruleTile;
        [SerializeField] protected TileType tileType;
        [SerializeField] protected bool isUnsafe;
        [SerializeField] protected bool isUnbreakable;

        [Min(1), PlayaDisableIf(nameof(isUnbreakable))]
        [SerializeField] protected int hardness = 50;

        [SerializeField] protected Ore ore;

        [Header("Sounds")]
        [SerializeField] protected AudioClip placeSound;
        [SerializeField] protected AudioClip hitSound;
        [SerializeField] protected AudioClip destroySound;

        public override InventoryModification UseOn(IWorld world, Vector3Int cell) => tileType switch
        {
            TileType.Block when IsUsableOn(world, cell) => world.PlaceTile(cell, this),
            _ => default
        };

        public override bool IsUsableOn(IWorld world, Vector3Int cell) => tileType switch
        {
            // TODO: maybe bring back this constraint (originally for cell highlighting)
            // bool notOccupiedByPlayer = !world.CellIntersects(cell, playerCollider.bounds);
            TileType.Block => world.GetBlock(cell) is null,
            _ => false
        };

        private void OnEnable()
        {
            if (ruleTile)
                ruleTile.WorldTile = this;
        }

        private void OnValidate()
        {
            if (ruleTile)
                ruleTile.WorldTile = this;
        }

        private void Reset()
        {
            maxAmount = 999;
            cooldown = 0.25f;
        }
    }
}
