using Tulip.Data;
using Tulip.Data.Items;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Player
{
    public class CellHighlighter : MonoBehaviour
    {
        [SerializeField] float speed = 100;
        [SerializeField] WorldModifier worldModifier;

        private new SpriteRenderer renderer;
        private Inventory inventory;
        private BoxCollider2D playerCollider;

        private Vector3 targetPosition;
        private Vector3Int? focusedCell;

        private void HandleCellFocusChanged(Vector3Int? cell) => focusedCell = cell;

        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            inventory = worldModifier.GetComponent<Inventory>();
            playerCollider = worldModifier.GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            if (!focusedCell.HasValue)
            {
                renderer.enabled = false;
                return;
            }

            WorldTile worldTile = World.Instance.GetTile(focusedCell.Value);
            Item item = inventory.HotbarSelected?.Item;
            bool notOccupiedByPlayer = !World.Instance.CellIntersects(focusedCell.Value, playerCollider.bounds);
            bool toolIsUsable = (item as Tool)?.IsUsableOnTile(worldTile) ?? false;

            renderer.enabled = item && item.Type switch
            {
                ItemType.Pickaxe => toolIsUsable,
                _ when item is WorldTile { TileType: TileType.Block } => toolIsUsable && notOccupiedByPlayer,
                _ => false
            };

            if (renderer.enabled)
                targetPosition = World.Instance.CellCenter(focusedCell.Value);
        }

        private void LateUpdate()
            => transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

        private void OnEnable() => worldModifier.OnChangeCellFocus += HandleCellFocusChanged;
        private void OnDisable() => worldModifier.OnChangeCellFocus -= HandleCellFocusChanged;
    }
}
