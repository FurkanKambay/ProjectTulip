using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tulip.GameWorld.Generation
{
    public class WorldGeneration : MonoBehaviour
    {
        [SerializeField] WorldData data;
        [SerializeField] Tilemap tilemap;
        [SerializeField] Tilemap backgroundTilemap;

        private float[,] PerlinNoise => perlinNoise ??= CalculateNoise();
        private float[,] perlinNoise;

        private void SetTiles()
        {
            for (int y = 0; y < data.height; y++)
            {
                float densityCutoff = data.heightDensityCurve.Evaluate(y / (float)data.height);

                for (int x = 0; x < data.width; x++)
                {
                    WorldTile tile = PerlinNoise[x, y] > densityCutoff ? null
                        : data.height - y < data.grassLayerHeight ? data.grass
                        : data.stone;

                    tilemap.SetTile(new Vector3Int(x, y, 0), tile ? tile.RuleTile : null);
                    backgroundTilemap.SetTile(new Vector3Int(x, y, 0), data.backgroundStone.RuleTile);
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
            float[,] noise = new float[data.width, data.height];
            float y = 0f;

            while (y < data.height)
            {
                float x = 0f;
                while (x < data.width)
                {
                    float sample = Mathf.PerlinNoise(
                        data.perlinOffset.x + (x / data.width * (data.width * data.densityFactor)),
                        data.perlinOffset.y + (y / data.height * (data.height * data.densityFactor)));

                    noise[(int)x, (int)y] = sample;
                    x++;
                }

                y++;
            }

            return noise;
        }

        private void ResetTilemapTransform()
        {
            tilemap.size = new Vector3Int(data.width, data.height, 1);
            tilemap.CompressBounds();
            tilemap.transform.position = new Vector3(-data.width / 2f, -data.height, 0);
            backgroundTilemap.transform.position = new Vector3(-data.width / 2f, -data.height, 0);
        }

        [ContextMenu("Reset Tilemap")]
        private void ResetTilemap()
        {
            tilemap.ClearAllTiles();
            ResetTilemapTransform();
        }
    }
}
