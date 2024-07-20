using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    public readonly struct TileModification
    {
        public readonly Vector3Int Cell;
        public readonly WorldTile WorldTile;
        public readonly TileModificationKind Kind;

        public static TileModification FromPlaced(Vector3Int cell, WorldTile worldTile) =>
            new(cell, worldTile, TileModificationKind.Placed);

        public static TileModification FromDamaged(Vector3Int cell, WorldTile worldTile) =>
            new(cell, worldTile, TileModificationKind.Damaged);

        public static TileModification FromDestroyed(Vector3Int cell, WorldTile worldTile) =>
            new(cell, worldTile, TileModificationKind.Destroyed);

        private TileModification(Vector3Int cell, WorldTile worldTile, TileModificationKind kind)
        {
            Cell = cell;
            WorldTile = worldTile;
            Kind = kind;
        }
    }
}
