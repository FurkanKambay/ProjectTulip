using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu(fileName = "WorldData", menuName = "Data/World", order = 0)]
    public class WorldGenConfig : ScriptableObject
    {
        [Header("Settings")]
        public int width = 100;
        public int height = 100;
        public Vector2 perlinOffset;

        [Header("Density Settings")]
        [Range(.02f, .25f)] public float densityFactor = .1f;
        public AnimationCurve heightDensityCurve;

        [Header("Blocks")]
        public WorldTile grass;
        public WorldTile stone;

        [Header("Background Tiles")]
        public WorldTile backgroundStone;

        [Header("Earth Layers")]
        public int grassLayerHeight = 10;
    }
}
