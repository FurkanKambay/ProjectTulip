using System;
using SaintsField;
using Tulip.Character;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Player
{
    public class Terraformer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] TangibleEntity entity;
        [SerializeField, Required] Inventory inventory;
        [SerializeField, Required] SaintsInterface<Component, IItemWielder> itemWielder;

        [Header("Config")]
        [SerializeField] float range = 5f;

        public event Action<Vector3Int?> OnChangeCellFocus;

        public Vector3Int MouseCell { get; private set; }

        public Vector3Int? FocusedCell
        {
            get => focusedCell;
            private set
            {
                if (focusedCell == value)
                    return;

                focusedCell = value;
                OnChangeCellFocus?.Invoke(focusedCell);
            }
        }

        private Vector3Int? focusedCell;
        private Vector2 rangePath;
        private Vector3 hitPoint;

        private void OnEnable() => itemWielder.I.OnSwing += HandleItemSwing;
        private void OnDisable() => itemWielder.I.OnSwing -= HandleItemSwing;

        private void Update()
        {
            if (itemWielder.I.CurrentStack.item is not WorldToolBase)
                return;

            AssignCells();
        }

        public bool IsCellBlockedByEntity()
        {
            if (!FocusedCell.HasValue)
                return false;

            Bounds bounds = entity.World.CellBoundsWorld(FocusedCell.Value);
            Vector2 topLeft = bounds.center - bounds.extents + (Vector3.one * 0.02f);
            Vector2 bottomRight = bounds.center + bounds.extents - (Vector3.one * 0.02f);

            int layerMask = LayerMask.GetMask("Enemy", "Player", "NPC", "Entity");
            return Physics2D.OverlapArea(topLeft, bottomRight, layerMask);
        }

        private void HandleItemSwing(ItemStack stack, Vector3 aimPoint)
        {
            if (!FocusedCell.HasValue || stack.item is not WorldToolBase tool)
                return;

            if (IsCellBlockedByEntity() || !tool.IsUsableOn(entity.World, FocusedCell.Value))
                return;

            InventoryModification modification = tool.UseOn(entity.World, FocusedCell.Value);
            InventoryModification remaining = inventory.ApplyModification(modification);

            if (!remaining.IsValid)
                return;

            Debug.LogWarning(
                $"[Terraformer] Remaining items: [{remaining.Stack}]"
                + (remaining.WouldAdd ? "not added" : "not removed")
            );
        }

        private void AssignCells()
        {
            Vector2 hotspot = transform.position;
            Vector2 aimPoint = hotspot + itemWielder.I.AimDirection;

            MouseCell = entity.World.WorldToCell(aimPoint);
            rangePath = Vector2.ClampMagnitude(itemWielder.I.AimDirection, range);

            if (entity.world.isReadonly)
            {
                FocusedCell = null;
                return;
            }

            if (!Options.Instance.Gameplay.UseSmartCursor || itemWielder.I.CurrentStack.item is not WorldTool)
            {
                float distance = Vector3.Distance(hotspot, aimPoint);
                FocusedCell = distance <= range ? MouseCell : null;
                return;
            }

            RaycastHit2D hit = Physics2D.Raycast(hotspot, rangePath, range, LayerMask.GetMask("World"));
            hitPoint = hit.point - (hit.normal * 0.1f);
            FocusedCell = hit.collider ? entity.World.WorldToCell(hitPoint) : null;
        }

        private void OnDrawGizmosSelected()
        {
            if (!Options.Instance.Gameplay.UseSmartCursor)
                return;

            Vector2 hotspot = transform.position;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(hotspot, hotspot + rangePath);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hitPoint, .1f);
        }
    }
}
