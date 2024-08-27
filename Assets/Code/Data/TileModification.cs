using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    public readonly struct TileModification
    {
        public readonly Vector2Int Cell;
        public readonly PlaceableData PlaceableData;
        public readonly TileModificationKind Kind;

        public static TileModification FromPlaced(Vector2Int cell, PlaceableData placeableData) =>
            new(cell, placeableData, TileModificationKind.Placed);

        public static TileModification FromDamaged(Vector2Int cell, PlaceableData placeableData) =>
            new(cell, placeableData, TileModificationKind.Damaged);

        public static TileModification FromDestroyed(Vector2Int cell, PlaceableData placeableData) =>
            new(cell, placeableData, TileModificationKind.Destroyed);

        private TileModification(Vector2Int cell, PlaceableData placeableData, TileModificationKind kind)
        {
            Cell = cell;
            PlaceableData = placeableData;
            Kind = kind;
        }
    }
}
