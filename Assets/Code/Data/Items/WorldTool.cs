using UnityEngine;

namespace Tulip.Data.Items
{
    /// <summary>
    /// A world tool such as a pickaxe.
    /// </summary>
    [CreateAssetMenu(menuName = "Items/World Tool")]
    public class WorldTool : WorldToolBase
    {
        public int Power => power;
        public TileType TileType => tileType;

        [Header("World Tool Data")]
        [SerializeField, Min(0)] protected int power = 50;
        [SerializeField] protected TileType tileType = TileType.Block;

        public override bool IsUsableOn(IWorld world, Vector3Int cell) =>
            world.HasBlock(cell) || world.HasEntity(cell);

        public override InventoryModification UseOn(IWorld world, Vector3Int cell) =>
            world.DamageTile(cell, tileType, power);
    }
}
