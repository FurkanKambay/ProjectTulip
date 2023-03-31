using Game.Data.Interfaces;
using Game.Data.Tiles;
using UnityEngine;

namespace Game.Data.Items
{
    [CreateAssetMenu(fileName = "Pickaxe", menuName = "Items/Pickaxe")]
    public class Pickaxe : ScriptableObject, ITool
    {
        public float Cooldown => cooldown;
        public Sprite Icon => icon;
        public float IconScale => iconScale;

        [SerializeField] private Sprite icon;
        [SerializeField] private float iconScale = 1f;

        [Header("Data")]
        public int power = 50;
        [SerializeField] private float cooldown = .5f;

        public bool CanUseOnBlock(BlockTile block) => block;
    }
}
