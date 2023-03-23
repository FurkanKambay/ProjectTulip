using System.Collections.Generic;
using Game.Items;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Player
{
    public class TileModifier : MonoBehaviour
    {
        public Tilemap Tilemap => tilemap;
        public Dictionary<Vector3Int, int> TileDamageMap { get; } = new();

        [SerializeField] private Tilemap tilemap;

        private float timeSinceLastUse;

        private void Update()
        {
            IUsable item = Inventory.Instance.HotbarSelected;

            timeSinceLastUse += Time.deltaTime;
            if (timeSinceLastUse < item.UseTime) return;

            if (!Input.Actions.Player.Fire.IsPressed()) return;
            timeSinceLastUse = 0;

            Vector2 mouse = Input.Instance.MouseWorldPoint;
            item.Use(this, tilemap.WorldToCell(mouse));
        }
    }
}
