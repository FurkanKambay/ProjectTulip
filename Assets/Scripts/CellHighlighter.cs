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
        private Vector3 targetPosition;

        private void Awake() => renderer = GetComponent<SpriteRenderer>();

        private void Update()
        {
            Vector2 mouse = Player.Input.Instance.MouseWorldPoint;
            Vector3Int cell = World.Instance.WorldToCell(mouse);
            bool hasBlock = World.Instance.HasBlock(cell);

            renderer.enabled = inventory.HotbarSelected switch {
                Pickaxe => hasBlock,
                BlockTile => !hasBlock,
                _ => false
            };

            targetPosition = World.Instance.CellCenter(cell);
        }

        private void LateUpdate()
            => transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
    }
}
