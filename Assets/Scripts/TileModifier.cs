using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Game
{
    public class TileModifier : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;

        [SerializeField] private Tile dirtTile;
        [SerializeField] private Tile stoneTile;
        [SerializeField] private Tile sandTile;

        private CompositeCollider2D compositeCollider;
        private new Camera camera;
        private Tile currentTile;

        private void Awake()
        {
            camera = Camera.main;
            compositeCollider = tilemap.GetComponent<CompositeCollider2D>();
            currentTile = dirtTile;
        }

        private void Update()
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                currentTile = dirtTile;
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
                currentTile = stoneTile;
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
                currentTile = sandTile;

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = camera.ScreenToWorldPoint(mousePosition);
            Vector3Int cellPos = tilemap.WorldToCell(worldPosition);

            if (Mouse.current.leftButton.isPressed)
                tilemap.SetTile(cellPos, currentTile);
            else if (Mouse.current.rightButton.isPressed)
                tilemap.SetTile(cellPos, null);
        }
    }
}
