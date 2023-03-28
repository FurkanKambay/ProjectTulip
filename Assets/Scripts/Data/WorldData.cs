using Game.Data.Tiles;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "WorldData", menuName = "Data/World", order = 0)]
    public class WorldData : ScriptableObject
    {
        [Header("Settings")]
        public int width = 100;
        public int height = 100;
        public Vector2 perlinOffset;

        [Header("Density Settings")]
        [Range(.02f, .25f)] public float densityFactor = .1f;
        public AnimationCurve heightDensityCurve;

        [Header("Blocks")]
        public BlockTile dirt;
        public BlockTile stone;
        public BlockTile deepstone;
        public BlockTile jungle;
        public BlockTile flesh;
        public BlockTile aquatic;

        [Header("Earth Layers")]
        public int dirtLayerHeight = 10;
        public int stoneLayerHeight = 10;

        [Header("Biomes")]
        public int aquaticBiomeWidth = 10;
        public int starterBiomeWidth = 10;
        public int jungleBiomeWidth = 10;
        public int fleshBiomeWidth = 10;
    }
}
