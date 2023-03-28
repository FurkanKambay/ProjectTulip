using Game.Data.Tiles;
using UnityEngine;

namespace Game.Data.Items
{
    [CreateAssetMenu(fileName = "Pickaxe", menuName = "Items/Pickaxe")]
    public class Pickaxe : ScriptableObject, IUsable
    {
        public float UseTime => useTime;
        public Sprite Icon => icon;
        public float IconScale => iconScale;

        [SerializeField] private Sprite icon;
        [SerializeField] private float iconScale = 1f;

        [Header("Data")]
        public int power = 50;
        [SerializeField] private float useTime = .5f;

        public bool CanUseOnBlock(BlockTile block) => block;
    }
}
