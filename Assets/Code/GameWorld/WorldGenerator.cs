using SaintsField;
using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.GameWorld
{
    public class WorldGenerator : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField, Expandable] WorldGenConfig config;

        public async Awaitable<WorldData> GenerateWorldAsync(string worldName)
        {
            if (string.IsNullOrWhiteSpace(worldName))
                return null;

#if !UNITY_WEBGL
            await Awaitable.BackgroundThreadAsync();
#endif

            float[,] perlinNoise = CalculatePerlinNoise();

            // int tileCount = config.Width * config.Height;
            var dimensions = new Vector2Int(config.Width, config.Height);

            var walls = new TileDictionary();
            var blocks = new TileDictionary();
            var curtains = new TileDictionary();

            int center = config.Width / 2;
            int snowDistance = center - config.SnowDistance;
            int sandDistance = center + config.SandDistance;

            for (int y = 0; y < config.Height; y++)
            {
                float densityCutoff = config.HeightDensityCurve.Evaluate(y / (float)config.Height);

                for (int x = 0; x < config.Width; x++)
                {
                    float noise = perlinNoise[x, y];

                    bool isUnderground = config.Height - y > config.GrassLayerHeight;
                    bool hasOre = noise < config.OreCutoff;

                    PlaceableData biomeSoil = x < snowDistance ? config.Snow
                        : x >= sandDistance ? config.Sand
                        : config.Grass;

                    PlaceableData block =
                        noise > densityCutoff ? null :
                        hasOre ? config.CopperVein :
                        !isUnderground ? biomeSoil :
                        config.Stone;

                    PlaceableData wall = config.StoneWall;
                    PlaceableData curtain = null;

                    var cell = new Vector2Int(x, y);

                    if (wall)
                        walls[cell] = wall;

                    if (block)
                        blocks[cell] = block;

                    if (curtain)
                        curtains[cell] = curtain;
                }
            }

            // walls.TrimExcess();
            // blocks.TrimExcess();
            // curtains.TrimExcess();

            var worldData = new WorldData(worldName, dimensions, walls, blocks, curtains);

            await Awaitable.MainThreadAsync();

            foreach (WorldGenConfig.StructureGen structure in config.Structures)
            {
                for (int i = 0; i < structure.amount; i++)
                {
                    // TODO: use seed for random structure position
                    var randomPosition = new Vector2Int(
                        x: Random.Range(0, dimensions.x),
                        y: Random.Range(0, dimensions.y)
                    );

                    WorldData structureData = structure.structureData.WorldData;
                    PlaceStructureTiles(structureData.Walls, worldData.Walls, randomPosition);
                    PlaceStructureTiles(structureData.Blocks, worldData.Blocks, randomPosition);
                    PlaceStructureTiles(structureData.Curtains, worldData.Curtains, randomPosition);
                }
            }

            return worldData;
        }

        private static void PlaceStructureTiles(TileDictionary source, TileDictionary target, Vector2Int pivotPosition)
        {
            foreach ((Vector2Int cell, PlaceableData placeableData) in source)
            {
                if (placeableData)
                    target[pivotPosition + cell] = placeableData;
                else
                    target.Remove(pivotPosition + cell);
            }
        }

        private float[,] CalculatePerlinNoise()
        {
            float[,] perlinNoise = new float[config.Width, config.Height];

            for (float y = 0; y < config.Height; y++)
            {
                for (float x = 0; x < config.Width; x++)
                {
                    float sample = Mathf.PerlinNoise(
                        config.PerlinOffset.x + (x / config.Width * (config.Width * config.densityFactor)),
                        config.PerlinOffset.y + (y / config.Height * (config.Height * config.densityFactor))
                    );

                    perlinNoise[(int)x, (int)y] = Mathf.Clamp01(sample);
                }
            }

            return perlinNoise;
        }
    }
}
