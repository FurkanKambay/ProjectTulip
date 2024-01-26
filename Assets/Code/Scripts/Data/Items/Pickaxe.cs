using Game.Data.Tiles;
using UnityEngine;

namespace Game.Data.Items
{
    /// <summary>
    /// A pickaxe.
    /// </summary>
    [CreateAssetMenu(fileName = "Pickaxe", menuName = "Items/Pickaxe")]
    public sealed class Pickaxe : Tool
    {
        public int Power => power;

        [Header("Pickaxe Data")]
        [SerializeField, Min(0)] int power = 50;

        public override bool IsUsableOnTile(WorldTile tile) => (bool)tile;
    }
}
