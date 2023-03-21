using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Items
{
    public class Pickaxe : IUsable
    {
        public void Use(Tilemap tilemap, Vector3Int cellPosition)
            => tilemap.SetTile(cellPosition, null);
    }
}
