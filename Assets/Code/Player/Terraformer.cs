using System;
using SaintsField;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Items;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Player
{
    public class Terraformer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, GetComponentInScene] World world;
        [SerializeField, Required] SaintsInterface<Component, IWielderBrain> brain;
        [SerializeField, Required] Inventory inventory;
        [SerializeField, Required] SaintsInterface<Component, IItemWielder> itemWielder;

        [Header("Config")]
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

        private Vector3Int? focusedCell;
        private Vector2 rangePath;
        private Vector3 hitPoint;

        private void OnEnable() => itemWielder.I.OnSwing += HandleItemSwing;
        private void OnDisable() => itemWielder.I.OnSwing -= HandleItemSwing;

        private void Update()
        {
            if (itemWielder.I.CurrentItem is not Tool) return;
            AssignCells();
        }

        private void HandleItemSwing(Usable item, Vector3 _)
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
            MouseCell = world.WorldToCell(brain.I.AimPosition);

            Vector2 hotspot = (Vector2)transform.position + hotspotOffset;
            rangePath = Vector2.ClampMagnitude(brain.I.AimPosition - hotspot, range);

            if (!Options.Instance.Gameplay.UseSmartCursor || itemWielder.I.CurrentItem is not Pickaxe)
            {
                float distance = Vector3.Distance(hotspot, brain.I.AimPosition);
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
    }
}
