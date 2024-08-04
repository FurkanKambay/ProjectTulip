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
        [SerializeField] Color validColor = Color.white;
        [SerializeField] Color invalidColor = Color.red;

        private Vector3 targetPosition;
        private Vector3Int? focusedCell;
        private Sprite defaultSprite;

        private void OnEnable() => terraformer.OnChangeCellFocus += HandleCellFocusChanged;
        private void OnDisable() => terraformer.OnChangeCellFocus -= HandleCellFocusChanged;

        private void Awake() => defaultSprite = renderer.sprite;

        private void Update()
        {
            if (!focusedCell.HasValue)
            {
                renderer.enabled = false;
                return;
            }

            Item item = itemWielder.I.CurrentStack.item;

            renderer.color = terraformer.IsCellBlockedByEntity() ? invalidColor : validColor;
            renderer.sprite = item is Placeable placeable ? placeable.Icon : defaultSprite;
            renderer.enabled = item is WorldToolBase worldTool && worldTool.IsUsableOn(world, focusedCell.Value);

            if (renderer.enabled)
                targetPosition = world.CellCenter(focusedCell.Value);
        }

        private void LateUpdate() =>
            transform.position = Vector3.Lerp(transform.position, targetPosition, trackingSpeed * Time.deltaTime);

        private void HandleCellFocusChanged(Vector3Int? cell) =>
            focusedCell = cell;
    }
}
