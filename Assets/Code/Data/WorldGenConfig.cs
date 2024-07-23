using SaintsField;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu]
    public class WorldGenConfig : ScriptableObject
    {
        public int Width => width;
        public int Height => height;
        public Vector2 PerlinOffset => perlinOffset;

        public AnimationCurve HeightDensityCurve => heightDensityCurve;
        public int GrassLayerHeight => grassLayerHeight;

        public WorldTile Grass => grass;
        public WorldTile Stone => stone;
        public WorldTile StoneWall => stoneWall;

        [Header("Settings")]
        [OverlayRichLabel("<color=grey>cells")]
        [SerializeField] int width = 100;

        [OverlayRichLabel("<color=grey>cells")]
        [SerializeField] int height = 100;

        [SerializeField] Vector2 perlinOffset;

        [Header("Density Settings")]
        [Range(.02f, .25f)] public float densityFactor = .1f;

        [SerializeField] AnimationCurve heightDensityCurve;

        [Header("Earth Layers")]
        [OverlayRichLabel("<color=grey>cells")]
        [SerializeField] int grassLayerHeight = 10;

        [Header("Blocks")]
        [SerializeField] WorldTile grass;

        [SerializeField] WorldTile stone;

        [Header("Walls")]
        [SerializeField] WorldTile stoneWall;
    }
}
