using System.Collections.Generic;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    using TileDictionary = Dictionary<Vector2Int, PlaceableData>;

    public class WorldData
    {
        public readonly Vector2Int Dimensions;
        public readonly TileDictionary Walls;
        public readonly TileDictionary Blocks;
        public readonly TileDictionary Curtains;

        public WorldData(Vector2Int dimensions, TileDictionary walls, TileDictionary blocks, TileDictionary curtains)
        {
            Dimensions = dimensions;
            Walls = walls;
            Blocks = blocks;
            Curtains = curtains;
        }
    }
}
