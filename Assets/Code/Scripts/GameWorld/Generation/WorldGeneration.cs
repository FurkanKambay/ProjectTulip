using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tulip.GameWorld.Generation
{
    public class WorldGeneration : MonoBehaviour
    {
        [SerializeField] WorldGenConfig config;
        [SerializeField] Tilemap tilemap;
        [SerializeField] Tilemap backgroundTilemap;

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

                    tilemap.SetTile(new Vector3Int(x, y, 0), tile ? tile.RuleTile : null);
                    backgroundTilemap.SetTile(new Vector3Int(x, y, 0), config.backgroundStone.RuleTile);
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

        private void ResetTilemapTransform()
        {
            tilemap.size = new Vector3Int(config.width, config.height, 1);
            tilemap.CompressBounds();
            tilemap.transform.position = new Vector3(-config.width / 2f, -config.height, 0);
            backgroundTilemap.transform.position = new Vector3(-config.width / 2f, -config.height, 0);
        }

        [ContextMenu("Reset Tilemap")]
        private void ResetTilemap()
        {
            tilemap.ClearAllTiles();
            ResetTilemapTransform();
        }
    }
}
