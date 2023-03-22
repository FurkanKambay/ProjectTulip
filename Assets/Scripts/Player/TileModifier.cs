using Game.Items;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Game.Player
{
    public class TileModifier : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;

        private void OnFire(InputAction.CallbackContext context)
        {
            IUsable selected = Inventory.Instance.HotbarSelected;
            Vector2 mouse = Input.Instance.MouseWorldPoint;
            selected?.Use(tilemap, tilemap.WorldToCell(mouse));
        }

        private void Start() => Input.Actions.Player.Fire.performed += OnFire;
    }
}
