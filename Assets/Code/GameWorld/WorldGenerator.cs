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
                    PlaceableData wall = config.StoneWall;
                    float noise = perlinNoise[x, y];

                    bool isGrassHeight = config.Height - y <= config.GrassLayerHeight;

                    PlaceableData block =
                        isGrassHeight && x < snowDistance ? config.Snow :
                        isGrassHeight && x >= sandDistance ? config.Sand :
                        noise > densityCutoff ? null :
                        noise < config.OreCutoff ? config.CopperVein :
                        isGrassHeight ? config.Grass :
                        config.Stone;

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

            await Awaitable.MainThreadAsync();

            return new WorldData(dimensions, walls, blocks, curtains);
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
