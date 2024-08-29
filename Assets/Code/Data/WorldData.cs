using System;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    [Serializable]
    public class TileDictionary : SerializableDictionary<Vector2Int, PlaceableData>
    {
    }

    [Serializable]
    public class WorldData
    {
        public Vector2Int Dimensions => dimensions;

        public TileDictionary Walls => walls;
        public TileDictionary Blocks => blocks;
        public TileDictionary Curtains => curtains;

        [SerializeField, Min(0)] private Vector2Int dimensions;
        [SerializeField] private TileDictionary walls;
        [SerializeField] private TileDictionary blocks;
        [SerializeField] private TileDictionary curtains;

        public WorldData(Vector2Int dimensions, TileDictionary walls, TileDictionary blocks, TileDictionary curtains)
        {
            this.dimensions = dimensions;
            this.walls = walls;
            this.blocks = blocks;
            this.curtains = curtains;
        }
    }
}
