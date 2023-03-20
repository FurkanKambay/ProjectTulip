using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Game
{
    public class TileModifier : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;

        private new Camera camera;
        private Inputs inputs;
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
                tilemap.SetTile(cellPosition, Inventory.Instance.HotbarSelectedTile);
            else if (Mouse.current.rightButton.isPressed)
                tilemap.SetTile(cellPosition, null);
        }

        private void Awake()
        {
            camera = Camera.main;
            inputs = new Inputs();
        }

        private void OnEnable()
        {
            inputs.Enable();
            inputs.Player.Point.performed += OnPoint;
        }

        private void OnDisable() => inputs.Disable();
    }
}
