using System.Collections.Generic;
using Game.Helpers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class World : Singleton<World>
    {
        public Tilemap Tilemap => tilemap;
        public Dictionary<Vector3Int, int> TileDamageMap { get; } = new();

        [SerializeField] private Tilemap tilemap;
    }
}
