using Tulip.Data;
using Tulip.Data.Items;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Player
{
    public class CellHighlighter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] World world;
        [SerializeField] WorldModifier worldModifier;
        [SerializeField] new SpriteRenderer renderer;

        [Header("Config")]
        [SerializeField] float trackingSpeed = 100;

        private IItemWielder itemWielder;

        private Vector3 targetPosition;
        private Vector3Int? focusedCell;

        private void Awake() => itemWielder = worldModifier.GetComponent<IItemWielder>();

        private void Update()
        {
            if (!focusedCell.HasValue)
            {
                renderer.enabled = false;
                return;
            }

            renderer.enabled = itemWielder.CurrentItem is Tool tool
                               && tool.IsUsableOn(world, focusedCell.Value);

            if (renderer.enabled)
                targetPosition = world.CellCenter(focusedCell.Value);
        }

        private void LateUpdate() =>
            transform.position = Vector3.Lerp(transform.position, targetPosition, trackingSpeed * Time.deltaTime);

        private void OnEnable() => worldModifier.OnChangeCellFocus += HandleCellFocusChanged;
        private void OnDisable() => worldModifier.OnChangeCellFocus -= HandleCellFocusChanged;

        private void HandleCellFocusChanged(Vector3Int? cell) => focusedCell = cell;
    }
}
