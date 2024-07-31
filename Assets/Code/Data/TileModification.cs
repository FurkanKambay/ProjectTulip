using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    public readonly struct TileModification
    {
        public readonly Vector3Int Cell;
        public readonly Placeable Placeable;
        public readonly TileModificationKind Kind;

        public static TileModification FromPlaced(Vector3Int cell, Placeable placeable) =>
            new(cell, placeable, TileModificationKind.Placed);

        public static TileModification FromDamaged(Vector3Int cell, Placeable placeable) =>
            new(cell, placeable, TileModificationKind.Damaged);

        public static TileModification FromDestroyed(Vector3Int cell, Placeable placeable) =>
            new(cell, placeable, TileModificationKind.Destroyed);

        private TileModification(Vector3Int cell, Placeable placeable, TileModificationKind kind)
        {
            Cell = cell;
            Placeable = placeable;
            Kind = kind;
        }
    }
}
