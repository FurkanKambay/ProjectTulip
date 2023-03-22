using Game.Items;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Player
{
    public class TileModifier : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;

        [Tooltip("Time in seconds between uses")]
        public float useTime = .5f;
        private float timeSinceLastUse;

        private void Update()
        {
            timeSinceLastUse += Time.deltaTime;
            if (timeSinceLastUse < useTime) return;

            if (!Input.Actions.Player.Fire.IsPressed()) return;
            timeSinceLastUse = 0;

            IUsable selected = Inventory.Instance.HotbarSelected;
            Vector2 mouse = Input.Instance.MouseWorldPoint;
            selected?.Use(tilemap, tilemap.WorldToCell(mouse));
        }
    }
}
