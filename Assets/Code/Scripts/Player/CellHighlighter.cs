using Game.Data;
using Game.Data.Interfaces;
using Game.Data.Items;
using Game.Data.Tiles;
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
        private Vector3Int? focusedCell;

        private World world;

        private void HandleCellFocusChanged(Vector3Int? cell) => focusedCell = cell;

        private void Awake()
        {
            world = World.Instance;
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

            WorldTile worldTile = world.GetTile(focusedCell.Value);
            IItem item = inventory.HotbarSelected?.Item;
            bool notOccupiedByPlayer = !world.CellIntersects(focusedCell.Value, playerCollider.bounds);
            bool toolIsUsable = (item as Tool)?.IsUsableOnTile(worldTile) ?? false;

            renderer.enabled = item?.Type switch
            {
                ItemType.Pickaxe => toolIsUsable,
                ItemType.Block => toolIsUsable && notOccupiedByPlayer,
                _ => false
            };

            if (renderer.enabled)
                targetPosition = world.CellCenter(focusedCell.Value);
        }

        private void LateUpdate()
            => transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

        private void OnEnable() => worldModifier.OnChangeCellFocus += HandleCellFocusChanged;
        private void OnDisable() => worldModifier.OnChangeCellFocus -= HandleCellFocusChanged;
    }
}
