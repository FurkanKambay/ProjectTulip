using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    public readonly struct TileModification
    {
        public readonly Vector2Int Cell;
        public readonly Placeable Placeable;
        public readonly TileModificationKind Kind;

        public static TileModification FromPlaced(Vector2Int cell, Placeable placeable) =>
            new(cell, placeable, TileModificationKind.Placed);

        public static TileModification FromDamaged(Vector2Int cell, Placeable placeable) =>
            new(cell, placeable, TileModificationKind.Damaged);

        public static TileModification FromDestroyed(Vector2Int cell, Placeable placeable) =>
            new(cell, placeable, TileModificationKind.Destroyed);

        private TileModification(Vector2Int cell, Placeable placeable, TileModificationKind kind)
        {
            Cell = cell;
            Placeable = placeable;
            Kind = kind;
        }
    }
}
