using Game.Data.Tiles;
using UnityEngine;

namespace Game
{
    public static class WorldExtensions
    {
        public static Vector3Int ToCell(this Vector3 worldPosition) => World.Instance.WorldToCell(worldPosition);
        public static bool HasTile(this Vector3Int cell) => World.Instance.HasTile(cell);
        public static WorldTile GetTile(this Vector3Int cell) => World.Instance.GetTile(cell);
        public static WorldTile GetTile(this Vector3 worldPosition) => World.Instance.GetTile(worldPosition);
    }
}
