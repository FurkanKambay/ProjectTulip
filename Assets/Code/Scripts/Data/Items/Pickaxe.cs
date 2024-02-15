using UnityEngine;

namespace Game.Data.Items
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

        public override bool IsUsableOnTile(WorldTile tile) => (bool)tile;
    }
}
