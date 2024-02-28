using System;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    public interface IWorld
    {
        event Action<Vector3Int, WorldTile> OnPlaceTile;
        event Action<Vector3Int, WorldTile> OnHitTile;
        event Action<Vector3Int, WorldTile> OnDestroyTile;
    }
}
