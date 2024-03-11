using Tulip.Data.Items;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Player
{
    public class CellHighlighter : MonoBehaviour
    {
        [SerializeField] float speed = 100;
        [SerializeField] WorldModifier worldModifier;

        private World world;
        private new SpriteRenderer renderer;
        private Inventory inventory;

        private Vector3 targetPosition;
        private Vector3Int? focusedCell;

        private void HandleCellFocusChanged(Vector3Int? cell) => focusedCell = cell;

        private void Awake()
        {
            world = FindAnyObjectByType<World>();
            renderer = GetComponent<SpriteRenderer>();
            inventory = worldModifier.GetComponent<Inventory>();
        }

        private void Update()
        {
            if (!focusedCell.HasValue)
            {
                renderer.enabled = false;
                return;
            }

            Item item = inventory.HotbarSelected?.Item;
            renderer.enabled = item is Tool tool
                               && tool.IsUsableOn(world, focusedCell.Value);

            if (renderer.enabled)
                targetPosition = world.CellCenter(focusedCell.Value);
        }

        private void LateUpdate()
            => transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

        private void OnEnable() => worldModifier.OnChangeCellFocus += HandleCellFocusChanged;
        private void OnDisable() => worldModifier.OnChangeCellFocus -= HandleCellFocusChanged;
    }
}
