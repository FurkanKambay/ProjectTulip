using System;
using SaintsField;
using Tulip.Character;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tulip.Player
{
    public class Terraformer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] TangibleEntity entity;
        [SerializeField, Required] Inventory inventory;
        [SerializeField, Required] SaintsInterface<Component, IItemWielder> itemWielder;

        [Header("Config")]
        [SerializeField] float coyoteTime = 0.15f;
        [SerializeField] float range = 5f;

        public event Action<Vector2Int?> OnChangeCellFocus;

        public Vector2Int MouseCell { get; private set; }

        public Vector2Int? FocusedCell
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

        private Vector2Int? focusedCell;
        private Vector2 rangePath;
        private Vector3 hitPoint;

        private ItemStack latestSwungStack;
        private float coyoteTimeCounter;

        private void OnEnable() => itemWielder.I.OnSwingPerform += ItemWielder_Swing;
        private void OnDisable() => itemWielder.I.OnSwingPerform -= ItemWielder_Swing;

        private void Update()
        {
            coyoteTimeCounter += Time.deltaTime;

            AssignCells();
            AttemptUse();
        }

        private void ItemWielder_Swing(ItemStack stack, Vector3 aimPoint)
        {
            latestSwungStack = stack;
            coyoteTimeCounter = 0;
        }

        /// Respects coyote time
        private void AttemptUse()
        {
            if (!latestSwungStack.IsValid || coyoteTimeCounter > coyoteTime)
                return;

            if (UseTool(latestSwungStack))
                latestSwungStack = default;
        }

        private bool UseTool(ItemStack stack)
        {
            if (!FocusedCell.HasValue || stack.itemData.IsNot(out BaseWorldToolData tool))
                return false;

            Assert.IsNotNull(tool);

            ToolUsability usability = tool.GetUsability(entity.World, FocusedCell.Value);

            if (usability != ToolUsability.Available)
                return false;

            InventoryModification modification = tool.UseOn(entity.World, FocusedCell.Value);

            // TODO: drop items
            inventory.ApplyModification(modification);

            return true;
        }

        private void AssignCells()
        {
            if (itemWielder.I.CurrentStack.itemData.IsNot(out BaseWorldToolData _))
                return;

            Vector2 hotspot = transform.position;
            Vector2 aimPoint = hotspot + itemWielder.I.AimDirection;

            MouseCell = entity.World.WorldToCell(aimPoint);
            rangePath = Vector2.ClampMagnitude(itemWielder.I.AimDirection, range);

            if (entity.world.IsReadonly)
            {
                FocusedCell = null;
                return;
            }

            if (!Options.Instance.Gameplay.UseSmartCursor || itemWielder.I.CurrentStack.itemData.IsNot(out WorldToolData _))
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
