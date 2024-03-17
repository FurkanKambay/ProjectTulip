using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.GameWorld.Generation
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] WorldGenConfig config;
        [SerializeField] World world;

        private float[,] PerlinNoise => perlinNoise ??= CalculateNoise();
        private float[,] perlinNoise;

        private void SetTiles()
        {
            for (int y = 0; y < config.height; y++)
            {
                float densityCutoff = config.heightDensityCurve.Evaluate(y / (float)config.height);

                for (int x = 0; x < config.width; x++)
                {
                    WorldTile tile = PerlinNoise[x, y] > densityCutoff ? null
                        : config.height - y < config.grassLayerHeight ? config.grass
                        : config.stone;

                    var cell = new Vector3Int(x, y, 0);
                    world.SetTile(cell, TileType.Block, tile);
                    world.SetTile(cell, TileType.Wall, config.backgroundStone);
                }
            }
        }

        [ContextMenu("Reassign All Tiles")]
        private void ReassignAll()
        {
            world.ResetTilemaps();
            SetTiles();
        }

        private void OnValidate()
        {
            if (Application.isEditor) return;
            SetTiles();
        }

        private void Start()
        {
            world.Size = new Vector2Int(config.width, config.height);
            ReassignAll();
        }

        private float[,] CalculateNoise()
        {
            float[,] noise = new float[config.width, config.height];
            float y = 0f;

            while (y < config.height)
            {
                float x = 0f;
                while (x < config.width)
                {
                    float sample = Mathf.PerlinNoise(
                        config.perlinOffset.x + (x / config.width * (config.width * config.densityFactor)),
                        config.perlinOffset.y + (y / config.height * (config.height * config.densityFactor)));

                    noise[(int)x, (int)y] = sample;
                    x++;
                }

                y++;
            }

            return noise;
        }
    }
}
