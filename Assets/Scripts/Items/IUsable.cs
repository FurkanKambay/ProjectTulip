using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Items
{
    public interface IUsable
    {
        void Use(Tilemap tilemap, Vector3Int cellPosition);
    }
}
