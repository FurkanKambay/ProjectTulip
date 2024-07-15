using SaintsField;
using Tulip.Data;
using Tulip.Data.Items;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Player
{
    public class CellHighlighter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] World world;
        [SerializeField, Required] Terraformer terraformer;
        [SerializeField, Required] SaintsInterface<Component, IItemWielder> itemWielder;
        [SerializeField] new SpriteRenderer renderer;

        [Header("Config")]
        [SerializeField] float trackingSpeed = 100;

        private Vector3 targetPosition;
        private Vector3Int? focusedCell;

        private void OnEnable() => terraformer.OnChangeCellFocus += HandleCellFocusChanged;
        private void OnDisable() => terraformer.OnChangeCellFocus -= HandleCellFocusChanged;

        private void Update()
        {
            if (!focusedCell.HasValue)
            {
                renderer.enabled = false;
                return;
            }

            renderer.enabled = itemWielder.I.CurrentStack.item is Tool tool
                               && tool.IsUsableOn(world, focusedCell.Value);

            if (renderer.enabled)
                targetPosition = world.CellCenter(focusedCell.Value);
        }

        private void LateUpdate() =>
            transform.position = Vector3.Lerp(transform.position, targetPosition, trackingSpeed * Time.deltaTime);

        private void HandleCellFocusChanged(Vector3Int? cell) =>
            focusedCell = cell;
    }
}
