using System;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Gameplay;
using Tulip.Data.Items;
using Tulip.GameWorld;
using Tulip.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Player
{
    public class WorldModifier : MonoBehaviour
    {
        [SerializeField] private InputHelper inputHelper;

        [SerializeField] Vector2 hotspotOffset;
        public float range = 5f;

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

        private World world;
        private Inventory inventory;
        private IItemWielder itemWielder;
        private Camera mainCamera;

        private Vector3Int? focusedCell;
        private Vector2 rangePath;
        private Vector3 hitPoint;

        private void Awake()
        {
            world = FindAnyObjectByType<World>();
            inventory = GetComponent<Inventory>();
            itemWielder = GetComponent<IItemWielder>();
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (itemWielder.CurrentItem is not Tool) return;
            AssignCells();
        }

        private void HandleItemSwing(Usable item, ItemSwingDirection _)
        {
            if (item is not Tool tool) return;

            if (!FocusedCell.HasValue) return;

            Bounds bounds = world.CellBoundsWorld(FocusedCell.Value);
            Vector2 topLeft = bounds.center - bounds.extents + (Vector3.one * 0.02f);
            Vector2 bottomRight = bounds.center + bounds.extents - (Vector3.one * 0.02f);

            int layerMask = LayerMask.GetMask("Enemy", "Player", "NPC");
            if (Physics2D.OverlapArea(topLeft, bottomRight, layerMask))
                return;

            if (!tool.IsUsableOn(world, FocusedCell.Value)) return;

            InventoryModification modification = tool.UseOn(world, FocusedCell.Value);
            inventory.ApplyModification(modification);
        }

        private void AssignCells()
        {
            Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(inputHelper.MouseScreenPoint);
            MouseCell = world.WorldToCell(mouseWorld);

            Vector2 hotspot = (Vector2)transform.position + hotspotOffset;
            rangePath = Vector2.ClampMagnitude(mouseWorld - hotspot, range);

            if (!Options.Instance.Gameplay.UseSmartCursor || inventory.HotbarSelected?.Item is not Pickaxe)
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
            if (!Options.Instance.Gameplay.UseSmartCursor) return;

            Vector2 hotspot = (Vector2)transform.position + hotspotOffset;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(hotspot, hotspot + rangePath);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hitPoint, .1f);
        }

        private void HandleToggleSmartCursor(InputAction.CallbackContext _)
            => Options.Instance.Gameplay.UseSmartCursor = !Options.Instance.Gameplay.UseSmartCursor;

        private void OnEnable()
        {
            inputHelper.Actions.Player.ToggleSmartCursor.performed += HandleToggleSmartCursor;
            itemWielder.OnSwing += HandleItemSwing;
        }

        private void OnDisable()
        {
            inputHelper.Actions.Player.ToggleSmartCursor.performed -= HandleToggleSmartCursor;
            itemWielder.OnSwing -= HandleItemSwing;
        }
    }
}
