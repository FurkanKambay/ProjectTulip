using UnityEngine;

namespace Game.Data.Items
{
    public interface IUsable
    {
        [Tooltip("Time in seconds between uses")]
        float UseTime { get; }
        Sprite Icon { get; }

        void Use(Vector3Int cell, Pickaxe pickaxe);
    }
}
