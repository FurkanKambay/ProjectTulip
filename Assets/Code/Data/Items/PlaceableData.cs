using SaintsField.Playa;
using Tulip.Data.Tiles;
using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/Placeable", order = 5)]
    public class PlaceableData : BaseWorldToolData
    {
        public override Sprite Icon => ruleTileData.m_DefaultSprite;

        public Color Color => color;
        public CustomRuleTileData RuleTileData => ruleTileData;
        public TileType TileType => tileType;
        public PlaceableMaterial Material => material;

        public bool IsUnsafe => isUnsafe;
        public bool IsUnbreakable => isUnbreakable;
        public int Hardness => hardness;
        public OreData OreData => oreData;

        [Header("World Tile Data")]
        [SerializeField] protected Color color;
        [SerializeField] protected CustomRuleTileData ruleTileData;
        [SerializeField] protected TileType tileType;
        [SerializeField] protected PlaceableMaterial material;

        [SerializeField] protected bool isUnsafe;
        [SerializeField] protected bool isUnbreakable;

        [Min(1), PlayaDisableIf(nameof(isUnbreakable))]
        [SerializeField] protected int hardness = 50;

        [SerializeField] protected OreData oreData;

        public override InventoryModification UseOn(IWorld world, Vector2Int cell) => tileType switch
        {
            TileType.Block when IsUsableOn(world, cell) => world.PlaceTile(cell, this),
            _ => default
        };

        public override bool IsUsableOn(IWorld world, Vector2Int cell) => tileType switch
        {
            // TODO: maybe bring back this constraint (originally for cell highlighting)
            // bool notOccupiedByPlayer = !world.CellIntersects(cell, playerCollider.bounds);
            TileType.Block => !world.HasBlock(cell),
            _ => false
        };

        private void OnEnable()
        {
            if (ruleTileData)
                ruleTileData.PlaceableData = this;
        }

        protected override void OnValidate()
        {
            if (ruleTileData)
                ruleTileData.PlaceableData = this;
        }

        private void Reset()
        {
            maxAmount = 999;
            cooldown = 0.25f;
        }
    }
}
