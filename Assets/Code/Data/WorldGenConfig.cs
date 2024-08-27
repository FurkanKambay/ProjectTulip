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

        public float OreCutoff => oreCutoff;
        public AnimationCurve HeightDensityCurve => heightDensityCurve;

        public int GrassLayerHeight => grassLayerHeight;
        public int SnowDistance => snowDistance;
        public int SandDistance => sandDistance;

        public PlaceableData Grass => grass;
        public PlaceableData Stone => stone;
        public PlaceableData Snow => snow;
        public PlaceableData Sand => sand;
        public PlaceableData CopperVein => copperVein;
        public PlaceableData StoneWall => stoneWall;

        [Header("Settings")]
        [OverlayRichLabel("<color=grey>cells")]
        [SerializeField] int width = 100;

        [OverlayRichLabel("<color=grey>cells")]
        [SerializeField] int height = 100;

        [SerializeField] Vector2 perlinOffset;

        [Header("Density Settings")]
        [Range(.02f, .25f)] public float densityFactor = 0.1f;

        [SerializeField, Range(0, 1)] float oreCutoff;
        [SerializeField] AnimationCurve heightDensityCurve;

        [Header("Earth Layers")]
        [OverlayRichLabel("<color=grey>cells")]
        [SerializeField, Min(0)] int grassLayerHeight = 10;

        [Header("Biomes")]
        [OverlayRichLabel("<color=grey>cells")]
        [SerializeField, Min(0)] int snowDistance;

        [OverlayRichLabel("<color=grey>cells")]
        [SerializeField, Min(0)] int sandDistance;

        [Header("Blocks")]
        [SerializeField] PlaceableData grass;
        [SerializeField] PlaceableData stone;
        [SerializeField] PlaceableData snow;
        [SerializeField] PlaceableData sand;
        [SerializeField] PlaceableData copperVein;

        [Header("Walls")]
        [SerializeField] PlaceableData stoneWall;

        private void OnValidate()
        {
            grassLayerHeight = Mathf.Min(grassLayerHeight, height);
            snowDistance = Mathf.Min(snowDistance, width / 2);
            sandDistance = Mathf.Min(sandDistance, width / 2);
        }
    }
}
