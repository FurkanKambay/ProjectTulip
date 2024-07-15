using System.Collections.Generic;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace Tulip.GameWorld.Generation
{
    public class WorldGenerator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] World world;

        [Header("Config")]
        [SerializeField] WorldGenConfig config;
        [SerializeField] UnityEvent onReady;

        private float[,] PerlinNoise => perlinNoise ??= CalculateNoise();
        private float[,] perlinNoise;

        List<TileChangeData> walls;
        List<TileChangeData> blocks;
        List<TileChangeData> curtains;

        private void OnEnable() => GameState.OnGameStateChange += HandleGameStateChange;
        private void OnDisable() => GameState.OnGameStateChange -= HandleGameStateChange;

        private async void Start() => await GenerateWorldData();

        public async void SetupWorld()
        {
            world.ResetTilemaps();
            world.SetTiles(TileType.Wall, walls.ToArray());
            world.SetTiles(TileType.Block, blocks.ToArray());
            world.SetTiles(TileType.Curtain, curtains.ToArray());
            world.isReadonly = false;

            await Awaitable.NextFrameAsync();
            onReady?.Invoke();
        }

        private async Awaitable GenerateWorldData()
        {
#if !UNITY_WEBGL
            await Awaitable.BackgroundThreadAsync();
#endif

            world.Size = new Vector2Int(config.width, config.height);
            int tileCount = config.width * config.height;

            walls = new List<TileChangeData>(tileCount);
            blocks = new List<TileChangeData>(tileCount);
            curtains = new List<TileChangeData>(tileCount);

            for (int y = 0; y < config.height; y++)
            {
                float densityCutoff = config.heightDensityCurve.Evaluate(y / (float)config.height);

                for (int x = 0; x < config.width; x++)
                {
                    WorldTile wall = config.backgroundStone;
                    WorldTile block = PerlinNoise[x, y] > densityCutoff ? null
                        : config.height - y < config.grassLayerHeight ? config.grass
                        : config.stone;

                    // int index = (config.width * y) + x;
                    var cell = new Vector3Int(x, y, 0);

                    walls.Add(new TileChangeData(cell, wall.RuleTile, wall.color, Matrix4x4.identity));

                    if (block)
                        blocks.Add(new TileChangeData(cell, block.RuleTile, block.color, Matrix4x4.identity));

                    curtains.Add(new TileChangeData(cell, null, Color.white, Matrix4x4.identity));
                }
            }

            walls.TrimExcess();
            blocks.TrimExcess();
            curtains.TrimExcess();

            await Awaitable.MainThreadAsync();
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

        private void HandleGameStateChange(GameState oldState, GameState newState)
        {
            if (oldState == GameState.MainMenu && newState == GameState.Playing)
                SetupWorld();
        }
    }
}
