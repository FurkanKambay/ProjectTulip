#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using SaintsField.Playa;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Items;
using Tulip.Data.Tiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tulip.GameWorld
{
    public class StructureEditor : MonoBehaviour
    {
        [Header("Tilemaps")]
        [SerializeField] Tilemap wallTilemap;
        [SerializeField] Tilemap blockTilemap;
        [SerializeField] Tilemap curtainTilemap;

        [Header("Structure Asset")]
        [SerializeField] StructureData structureData;

        [Header("Config")]
        [SerializeField] bool serializeNullTiles;

        private WorldData WorldData => structureData.WorldData;

        private bool AreTilemapsDirty => EditorUtility.IsDirty(wallTilemap)
                                         && EditorUtility.IsDirty(blockTilemap)
                                         && EditorUtility.IsDirty(curtainTilemap);

        private bool AreTilemapsUsed => wallTilemap.GetUsedTilesCount() > 0
                                        || blockTilemap.GetUsedTilesCount() > 0
                                        || curtainTilemap.GetUsedTilesCount() > 0;

        // ReSharper disable once UnusedMember.Local
        [Button]
        private void SaveToAsset()
        {
            // TODO: draw hard bounds in scene

            ResizeTilemap(wallTilemap);
            ResizeTilemap(blockTilemap);
            ResizeTilemap(curtainTilemap);

            Vector2Int dimensions = WorldData.Dimensions;
            // int cellCount = dimensions.x * dimensions.y;

            var walls = new TileDictionary();
            var blocks = new TileDictionary();
            var curtains = new TileDictionary();

            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    var cell = new Vector2Int(x, y);

                    CustomRuleTileData wallTile = wallTilemap.GetTile<CustomRuleTileData>((Vector3Int)cell);
                    CustomRuleTileData blockTile = blockTilemap.GetTile<CustomRuleTileData>((Vector3Int)cell);
                    CustomRuleTileData curtainTile = curtainTilemap.GetTile<CustomRuleTileData>((Vector3Int)cell);

                    if (wallTile)
                        walls[cell] = wallTile.PlaceableData;
                    else if (serializeNullTiles)
                        walls[cell] = null;

                    if (blockTile)
                        blocks[cell] = blockTile.PlaceableData;
                    else if (serializeNullTiles)
                        blocks[cell] = null;

                    if (curtainTile)
                        curtains[cell] = curtainTile.PlaceableData;
                    else if (serializeNullTiles)
                        curtains[cell] = null;
                }
            }

            structureData.SetWorldData(new WorldData(dimensions, walls, blocks, curtains));
            EditorUtility.SetDirty(structureData);
        }

        // ReSharper disable once UnusedMember.Local
        [Button]
        private void RevertToAsset()
        {
            ResetTilemaps();

            if (WorldData.Walls != null)
                wallTilemap.SetTiles(WorldData.Walls.Select(selector).ToArray(), ignoreLockFlags: true);

            if (WorldData.Blocks != null)
                blockTilemap.SetTiles(WorldData.Blocks.Select(selector).ToArray(), ignoreLockFlags: true);

            if (WorldData.Curtains != null)
                curtainTilemap.SetTiles(WorldData.Curtains.Select(selector).ToArray(), ignoreLockFlags: true);

            return;

            TileChangeData selector(KeyValuePair<Vector2Int, PlaceableData> kvp)
            {
                (Vector2Int cell, PlaceableData placeableData) = kvp;

                return new TileChangeData(
                    (Vector3Int)cell,
                    (bool)placeableData ? placeableData.RuleTileData : null,
                    (bool)placeableData ? placeableData.Color : Color.white,
                    Matrix4x4.identity
                );
            }
        }

        [Button, PlayaEnableIf(nameof(AreTilemapsUsed))]
        private void ResetTilemaps()
        {
            ResetTilemap(wallTilemap);
            ResetTilemap(blockTilemap);
            ResetTilemap(curtainTilemap);
            MarkSceneAsDirty();
        }

        private void ResetTilemap(Tilemap tilemap)
        {
            tilemap.ClearAllTiles();
            ResizeTilemap(tilemap);
        }

        private void ResizeTilemap(Tilemap tilemap)
        {
            tilemap.size = WorldData.Dimensions.WithZ(1);
            tilemap.ResizeBounds();
        }

        private void MarkSceneAsDirty()
        {
            EditorUtility.SetDirty(wallTilemap);
            EditorUtility.SetDirty(blockTilemap);
            EditorUtility.SetDirty(curtainTilemap);
        }

        private void UnmarkSceneAsDirty()
        {
            EditorUtility.ClearDirty(wallTilemap);
            EditorUtility.ClearDirty(blockTilemap);
            EditorUtility.ClearDirty(curtainTilemap);
        }

        private void OnDrawGizmos()
        {
            Vector2Int dimensions = WorldData.Dimensions;

            var center = new Vector3(dimensions.x / 2f, dimensions.y / 2f);
            var size = new Vector3(dimensions.x + 1f, dimensions.y + 1f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center, size);
        }
    }
}

#endif
