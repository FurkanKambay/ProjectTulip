using Game.Data.Interfaces;
using Game.Data.Tiles;
using UnityEngine;

namespace Game.Data.Items
{
    [CreateAssetMenu(fileName = "Pickaxe", menuName = "Items/Pickaxe")]
    public sealed class Pickaxe : Usable, ITool
    {
        public int Power => power;

        [Header("Pickaxe Data")]
        [SerializeField, Min(0)] int power = 50;

        public bool CanUseOnBlock(BlockTile block) => block;
    }
}
