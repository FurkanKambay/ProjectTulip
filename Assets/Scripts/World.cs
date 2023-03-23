using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class World : MonoBehaviour
    {
        public static World Instance { get; private set; }

        public Tilemap Tilemap => tilemap;
        public Dictionary<Vector3Int, int> TileDamageMap { get; } = new();

        [SerializeField] private Tilemap tilemap;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }
    }
}
