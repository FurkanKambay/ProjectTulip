using Game.Data.Interfaces;
using UnityEngine;

namespace Game.Data.Items
{
    [CreateAssetMenu(fileName = "Pickaxe", menuName = "Items/Pickaxe")]
    public class Pickaxe : Tool
    {
        [Header("Pickaxe Data")]
        public int power = 50;
    }
}
