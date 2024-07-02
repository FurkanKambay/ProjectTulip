using Tulip.Data.Tiles;
using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(fileName = "World Tile", menuName = "Items/World Tile")]
    public class WorldTile : Tool
    {
        public bool IsSafe => true;
        public virtual TileType TileType => tileType;
        public CustomRuleTile RuleTile => ruleTile;

        public override Sprite Icon => ruleTile.m_DefaultSprite;
        public override float Cooldown => 0.25f;

        [Header("Tile Data")]
        [SerializeField] TileType tileType;
        [SerializeField] CustomRuleTile ruleTile;
        public Color color = Color.white;

        [Header("World Tile Data")]
        [Min(1)] public int hardness = 50;
        public bool isUnbreakable;

        [Header("Sounds")]
        public AudioClip hitSound;
        public AudioClip destroySound;
        public AudioClip placeSound;

        public override InventoryModification UseOn(IWorld world, Vector3Int cell) => tileType switch
        {
            TileType.Block when IsUsableOn(world, cell) => world.PlaceTile(cell, this),
            _ => InventoryModification.Empty
        };

        public override bool IsUsableOn(IWorld world, Vector3Int cell) => tileType switch
        {
            // TODO: maybe bring back this constraint (originally for cell highlighting)
            // bool notOccupiedByPlayer = !world.CellIntersects(cell, playerCollider.bounds);
            TileType.Block => world.GetTile(cell) is null,
            _ => false
        };

        private void OnEnable() => RuleTile.WorldTile = this;
        private void OnValidate() => RuleTile.WorldTile = this;

        private void Reset() => maxAmount = 999;
    }
}
