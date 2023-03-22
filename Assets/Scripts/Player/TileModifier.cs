using Game.Items;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Game.Player
{
    public class TileModifier : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;

        private new Camera camera;
        private InputActions input;
        private Vector3Int cellPosition;

        public void OnPoint(InputAction.CallbackContext context)
        {
            Vector2 mousePosition = context.ReadValue<Vector2>();
            Vector3 worldPosition = camera.ScreenToWorldPoint(mousePosition);
            cellPosition = tilemap.WorldToCell(worldPosition);
        }

        private void Update()
        {
            if (Mouse.current.leftButton.isPressed)
            {
                IUsable selected = Inventory.Instance.HotbarSelected;
                selected?.Use(tilemap, cellPosition);
            }
        }

        private void Awake()
        {
            camera = Camera.main;
            input = new InputActions();
            input.Player.Point.performed += OnPoint;
        }

        private void OnEnable() => input.Enable();
        private void OnDisable() => input.Disable();
    }
}
