using Game.Player;
using UnityEngine;

namespace Game.Items
{
    public interface IUsable
    {
        [Tooltip("Time in seconds between uses")]
        float UseTime { get; }

        void Use(TileModifier modifier, Vector3Int cellPosition);
    }
}
