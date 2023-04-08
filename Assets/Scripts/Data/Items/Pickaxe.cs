using Game.Data.Interfaces;
using Game.Data.Tiles;
using UnityEngine;

namespace Game.Data.Items
{
    [CreateAssetMenu(fileName = "Pickaxe", menuName = "Items/Pickaxe")]
    public sealed class Pickaxe : Usable, ITool
    {
        [Header("Pickaxe Data")]
        public int power = 50;

        public bool CanUseOnBlock(BlockTile block) => block;
    }
}
