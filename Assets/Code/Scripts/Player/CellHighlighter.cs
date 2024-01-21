using Game.Data;
using UnityEngine;

namespace Game.Player
{
    public class CellHighlighter : MonoBehaviour
    {
        [SerializeField] float speed = 100;
        [SerializeField] WorldModifier worldModifier;

        private new SpriteRenderer renderer;
        private Inventory inventory;
        private BoxCollider2D playerCollider;

        private Vector3 targetPosition;
        private Vector3Int? highlightedCell;

        private World world;

        private void HandleCellFocusChanged(Vector3Int? cell) => highlightedCell = cell;

        private void Awake()
        {
            world = World.Instance;
            renderer = GetComponent<SpriteRenderer>();
            inventory = worldModifier.GetComponent<Inventory>();
            playerCollider = worldModifier.GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            Vector3Int cell = highlightedCell.GetValueOrDefault();
            bool hasTile = world.HasTile(cell);
            bool notOccupied = !world.CellIntersects(cell, playerCollider.bounds);

            renderer.enabled = highlightedCell.HasValue && inventory.HotbarSelected?.Item?.Type switch
            {
                ItemType.Pickaxe => hasTile,
                ItemType.Block => !hasTile && notOccupied,
                _ => false
            };

            if (!renderer.enabled) return;
            targetPosition = world.CellCenter(cell);
        }

        private void LateUpdate()
            => transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

        private void OnEnable() => worldModifier.OnChangeCellFocus += HandleCellFocusChanged;
        private void OnDisable() => worldModifier.OnChangeCellFocus -= HandleCellFocusChanged;
    }
}
