using System;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    public interface IWorld
    {
        event Action<TileModification> OnPlaceTile;
        event Action<TileModification> OnHitTile;
        event Action<TileModification> OnDestroyTile;
    }
}
