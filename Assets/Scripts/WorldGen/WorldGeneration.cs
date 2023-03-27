using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.WorldGen
{
    public class WorldGeneration : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Tilemap tilemap;

        [SerializeField] private TileBase dirt;
        [SerializeField] private TileBase stone;
        [SerializeField] private TileBase deepstone;
        [SerializeField] private TileBase jungle;
        [SerializeField] private TileBase flesh;
        [SerializeField] private TileBase aquatic;

        [Header("Settings")]
        public int width = 100;
        public int height = 100;
        public Vector2 perlinOffset;

        [Header("Density Settings")]
        [Range(.02f, .25f)] public float densityFactor = .1f;
        [SerializeField] private AnimationCurve heightDensityCurve;

        [Header("Earth Layers")]
        public int dirtLayerHeight = 10;
        public int stoneLayerHeight = 10;

        [Header("Biomes")]
        public int aquaticBiomeWidth = 10;
        public int starterBiomeWidth = 10;
        public int jungleBiomeWidth = 10;
        public int fleshBiomeWidth = 10;

        private float[,] PerlinNoise => perlinNoise ??= CalculateNoise();
        private float[,] perlinNoise;

        private void SetTiles()
        {
            for (int y = 0; y < height; y++)
            {
                float densityCutoff = heightDensityCurve.Evaluate(y / (float)height);

                for (int x = 0; x < width; x++)
                {
                    TileBase biomeSoil = x < aquaticBiomeWidth ? aquatic
                        : x < aquaticBiomeWidth + starterBiomeWidth ? dirt
                        : x < aquaticBiomeWidth + starterBiomeWidth + jungleBiomeWidth ? jungle
                        : x < aquaticBiomeWidth + starterBiomeWidth + jungleBiomeWidth + fleshBiomeWidth ? flesh
                        : dirt;

                    TileBase tile = PerlinNoise[x, y] > densityCutoff ? null
                        : height - y < dirtLayerHeight ? biomeSoil
                        : height - y < stoneLayerHeight ? stone
                        : deepstone;

                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }

        [ContextMenu("Reassign All Tiles")]
        private void ReassignAll()
        {
            tilemap.ClearAllTiles();
            ResetTilemapTransform();
            SetTiles();
        }

        private void OnValidate()
        {
            if (Application.isEditor) return;

            ResetTilemapTransform();
            SetTiles();
        }

        private void Start() => ReassignAll();

        private float[,] CalculateNoise()
        {
            float[,] noise = new float[width, height];
            float y = 0f;

            while (y < height)
            {
                float x = 0f;
                while (x < width)
                {
                    float sample = Mathf.PerlinNoise(
                        perlinOffset.x + (x / width * (width * densityFactor)),
                        perlinOffset.y + (y / height * (height * densityFactor)));

                    noise[(int)x, (int)y] = sample;
                    x++;
                }

                y++;
            }

            return noise;
        }

        private void ResetTilemapTransform()
        {
            tilemap.size = new Vector3Int(width, height, 1);
            tilemap.CompressBounds();
            tilemap.transform.position = new Vector3(-width / 2f, -height, 0);
        }

        [ContextMenu("Reset Tilemap")]
        private void ResetTilemap()
        {
            tilemap.ClearAllTiles();
            ResetTilemapTransform();
        }
    }
}
