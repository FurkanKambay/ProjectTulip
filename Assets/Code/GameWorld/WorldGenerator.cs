using SaintsField;
using SaintsField.Playa;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.GameWorld
{
    public class WorldGenerator : MonoBehaviour, IWorldProvider
    {
        [Header("References")]
        [SerializeField] World world;

        [Header("Config")]
        [SerializeField, Expandable] WorldGenConfig config;

        public event IWorldProvider.ProvideWorldEvent OnProvideWorld;

        private void OnEnable() => GameManager.OnGameStateChange += HandleGameStateChange;
        private void OnDisable() => GameManager.OnGameStateChange -= HandleGameStateChange;

        private async void HandleGameStateChange(GameState oldState, GameState newState)
        {
            if (oldState == GameState.MainMenu && newState == GameState.Playing)
                await ApplyWorldAsync();
        }

        [Button]
        private async Awaitable ApplyWorldAsync()
        {
            WorldData worldData = await GenerateDataAsync();
            OnProvideWorld?.Invoke(worldData);
        }

        private async Awaitable<WorldData> GenerateDataAsync()
        {
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

            var worldData = new WorldData(dimensions, walls, blocks, curtains);

            await Awaitable.MainThreadAsync();

            foreach (WorldGenConfig.StructureGen structureGenConfig in config.Structures)
            {
                for (int i = 0; i < structureGenConfig.amount; i++)
                {
                    var randomPosition = new Vector2Int(
                        x: Random.Range(0, dimensions.x),
                        y: Random.Range(0, dimensions.y)
                    );

                    PlaceStructure(worldData, structureGenConfig.structureData, randomPosition);
                }
            }

            return worldData;
        }

        private void PlaceStructure(WorldData worldData, StructureData structureData, Vector2Int pivotPosition)
        {
            foreach ((Vector2Int cell, PlaceableData placeableData) in structureData.WorldData.Walls)
                worldData.Walls[pivotPosition + cell] = placeableData;

            foreach ((Vector2Int cell, PlaceableData placeableData) in structureData.WorldData.Blocks)
                worldData.Blocks[pivotPosition + cell] = placeableData;

            foreach ((Vector2Int cell, PlaceableData placeableData) in structureData.WorldData.Curtains)
                worldData.Curtains[pivotPosition + cell] = placeableData;
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
