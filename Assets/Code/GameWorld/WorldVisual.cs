using System;
using System.Collections.Generic;
using System.Linq;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tulip.GameWorld
{
    public class WorldVisual : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] World world;
        [SerializeField] Tilemap wallTilemap;
        [SerializeField] Tilemap blockTilemap;
        [SerializeField] Tilemap curtainTilemap;

        private void OnEnable()
        {
            world.OnRefresh += HandleRefreshWorld;
            world.OnPlaceTile += HandlePlaceTile;
            world.OnDestroyTile += HandleDestroyTile;
        }

        private void OnDisable()
        {
            world.OnRefresh -= HandleRefreshWorld;
            world.OnPlaceTile -= HandlePlaceTile;
            world.OnDestroyTile -= HandleDestroyTile;
        }

        public Vector2Int WorldToCell(Vector3 worldPosition) =>
            (Vector2Int)blockTilemap.WorldToCell(worldPosition);

        public Vector3 GetCellCenterWorld(Vector2Int cell) =>
            blockTilemap.GetCellCenterWorld((Vector3Int)cell);

        public Bounds CellBoundsWorld(Vector2Int cell) =>
            new(GetCellCenterWorld(cell), blockTilemap.GetBoundsLocal((Vector3Int)cell).size);

        private void HandleRefreshWorld(WorldData worldData)
        {
            resetTilemap(wallTilemap);
            resetTilemap(blockTilemap);
            resetTilemap(curtainTilemap);

            // TODO: Improve performance here
            wallTilemap.SetTiles(world.WorldData.Walls.Select(selector).ToArray(), ignoreLockFlags: true);
            blockTilemap.SetTiles(world.WorldData.Blocks.Select(selector).ToArray(), ignoreLockFlags: true);
            curtainTilemap.SetTiles(world.WorldData.Curtains.Select(selector).ToArray(), ignoreLockFlags: true);
            return;

            void resetTilemap(Tilemap tilemap)
            {
                tilemap.ClearAllTiles();
                tilemap.size = world.Dimensions.WithZ(1);
                tilemap.transform.position = new Vector3(-world.Dimensions.x / 2f, -world.Dimensions.y, 0);
            }

            TileChangeData selector(KeyValuePair<Vector2Int, Placeable> kvp)
            {
                (Vector2Int cell, Placeable placeable) = kvp;

                return new TileChangeData(
                    (Vector3Int)cell,
                    (bool)placeable ? placeable.RuleTile : null,
                    (bool)placeable ? placeable.Color : Color.white,
                    Matrix4x4.identity
                );
            }
        }

        private void HandlePlaceTile(TileModification modification)
        {
            var cell = (Vector3Int)modification.Cell;
            Placeable placeable = modification.Placeable;

            Tilemap tilemap = GetTilemap(placeable.TileType);
            tilemap.SetTile(cell, placeable.RuleTile);

            if (!placeable)
            {
                tilemap.SetTile(cell, null);
                return;
            }

            tilemap.SetTile(cell, placeable.RuleTile);
            tilemap.SetColor(cell, placeable.Color);
        }

        private void HandleDestroyTile(TileModification modification)
        {
            Tilemap tilemap = GetTilemap(modification.Placeable.TileType);
            tilemap.SetTile((Vector3Int)modification.Cell, null);
        }

        private Tilemap GetTilemap(TileType tileType) => tileType switch
        {
            TileType.Wall => wallTilemap,
            TileType.Block => blockTilemap,
            TileType.Curtain => curtainTilemap,
            _ => throw new ArgumentOutOfRangeException(nameof(tileType))
        };
    }
}
