using UnityEngine;

namespace Tulip.Data.Items
{
    /// <summary>
    /// A pickaxe.
    /// </summary>
    [CreateAssetMenu(fileName = "Pickaxe", menuName = "Items/Pickaxe")]
    public class Pickaxe : Tool
    {
        public virtual int Power => power;

        [Header("Pickaxe Data")]
        [SerializeField, Min(0)] protected int power = 50;

        public override bool IsUsableOn(IWorld world, Vector3Int cell)
            => (bool)world.GetBlock(cell);

        public override InventoryModification UseOn(IWorld world, Vector3Int cell)
            => world.DamageTile(cell, TileType.Block, Power);
    }
}
