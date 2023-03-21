using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Items
{
    public class Block : IUsable
    {
        public Tile Tile;

        public void Use(Tilemap tilemap, Vector3Int cellPosition)
            => tilemap.SetTile(cellPosition, Tile);
    }
}
