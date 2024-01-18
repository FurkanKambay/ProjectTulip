using System;
using Game.Data;
using Game.Data.Interfaces;
using Game.Data.Items;
using Game.Data.Tiles;
using Game.Input;
using UnityEngine;

namespace Game.Player
{
    public class WorldModifier : MonoBehaviour
    {
        [SerializeField] Vector2 hotspotOffset;
        public float range = 5f;
        public bool smartCursor;

        public Vector3Int MouseCell { get; private set; }

        public Vector3Int? FocusedCell
        {
            get => focusedCell;
            private set
            {
                if (focusedCell == value) return;
                focusedCell = value;
                OnChangeCellFocus?.Invoke(focusedCell);
            }
        }

        public event Action<Vector3Int?> OnChangeCellFocus;

        private Inventory inventory;
        private BoxCollider2D playerCollider;

        private float timeSinceLastUse;
        private Vector3Int? focusedCell;
        private Vector2 rangePath;
        private Vector3 hitPoint;

        private InputHelper input;
        private World world;

        private void Awake()
        {
            input = InputHelper.Instance;
            world = World.Instance;

            InputHelper.Actions.Player.ToggleSmartCursor.performed += _ => smartCursor = !smartCursor;

            inventory = GetComponent<Inventory>();
            playerCollider = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            var item = inventory.HotbarSelected?.Item as ITool;
            AssignCells();

            timeSinceLastUse += Time.deltaTime;
            if (item == null || timeSinceLastUse <= item.Cooldown) return;

            if (!FocusedCell.HasValue) return;
            if (world.CellIntersects(FocusedCell.Value, playerCollider.bounds)) return;
            if (!InputHelper.Actions.Player.Use.IsPressed()) return;

            timeSinceLastUse = 0;

            BlockTile blockTile = world.GetBlock(FocusedCell.Value);
            if (!item.CanUseOnBlock(blockTile)) return;

            inventory.ApplyModification(item.Type switch
            {
                ItemType.Block => world.PlaceBlock(FocusedCell.Value, item as BlockTile),
                ItemType.Wall => InventoryModification.Empty,
                ItemType.Pickaxe => world.DamageBlock(FocusedCell.Value, ((Pickaxe)item).Power),
                _ => InventoryModification.Empty
            });
        }

        private void AssignCells()
        {
            Vector3 mouseWorld = input.MouseWorldPoint;
            MouseCell = world.WorldToCell(mouseWorld);

            Vector2 hotspot = (Vector2)transform.position + hotspotOffset;
            rangePath = Vector2.ClampMagnitude((Vector2)mouseWorld - hotspot, range);

            if (!smartCursor || inventory.HotbarSelected?.Item is not Pickaxe)
            {
                float distance = Vector3.Distance(hotspot, mouseWorld);
                FocusedCell = distance <= range ? MouseCell : null;
                return;
            }

            RaycastHit2D hit = Physics2D.Raycast(
                hotspot, rangePath, range,
                LayerMask.GetMask("World"));

            hitPoint = hit.point - (hit.normal * 0.1f);
            FocusedCell = hit.collider ? world.WorldToCell(hitPoint) : null;
        }

        private void OnDrawGizmosSelected()
        {
            if (!smartCursor) return;

            Vector2 hotspot = (Vector2)transform.position + hotspotOffset;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(hotspot, hotspot + rangePath);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hitPoint, .1f);
        }
    }
}
