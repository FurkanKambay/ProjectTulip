using Game.Data.Items;
using Game.Data.Tiles;
using Game.Player;
using UnityEngine;

namespace Game
{
    public class CellHighlighter : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private float speed = 100;

        private new SpriteRenderer renderer;
        private WorldModifier worldModifier;
        private Vector3 targetPosition;
        private Vector3Int highlightedCell;

        private void OnCellFocusChanged(Vector3Int cell)
            => highlightedCell = cell;

        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            worldModifier = inventory.GetComponent<WorldModifier>();
        }

        private void Update()
        {
            bool hasBlock = World.Instance.HasBlock(highlightedCell);

            renderer.enabled = inventory.HotbarSelected switch {
                Pickaxe => hasBlock,
                BlockTile => !hasBlock,
                _ => false
            };

            if (!renderer.enabled) return;
            targetPosition = World.Instance.CellCenter(highlightedCell);
        }

        private void LateUpdate()
            => transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

        private void OnEnable() => worldModifier.CellFocusChanged += OnCellFocusChanged;
        private void OnDisable() => worldModifier.CellFocusChanged -= OnCellFocusChanged;
    }
}
