using System;
using Game.Data.Interfaces;
using Game.Data.Items;
using Game.Data.Tiles;
using UnityEngine;

namespace Game.Player
{
    public class WorldModifier : MonoBehaviour
    {
        [SerializeField] Vector2 hotspotOffset;
        public float range = 5f;
        public bool smartCursor;

        public Vector3Int MouseCell { get; private set; }

        public Vector3Int? FocusedCell
        {
            get => focusedCell;
            private set
            {
                if (focusedCell == value) return;
                focusedCell = value;
                CellFocusChanged?.Invoke(focusedCell);
            }
        }

        public event Action<Vector3Int?> CellFocusChanged;

        private Inventory inventory;
        private AudioSource audioSource;
        private BoxCollider2D playerCollider;

        private float timeSinceLastUse;
        private Vector3Int? focusedCell;
        private Vector2 rangePath;
        private Vector3 hitPoint;

        private Input input;
        private World world;

        private void Awake()
        {
            input = Input.Instance;
            world = World.Instance;

            Input.Actions.Player.ToggleSmartCursor.performed += _ => smartCursor = !smartCursor;

            inventory = GetComponent<Inventory>();
            audioSource = GetComponent<AudioSource>();
            playerCollider = GetComponent<BoxCollider2D>();

            world.BlockPlaced += PlayPlaceSound;
            world.BlockHit += PlayHitSound;
            world.BlockDestroyed += PlayHitSound;
        }

        private void Update()
        {
            var item = inventory.HotbarSelected?.Item as ITool;
            AssignCells();

            timeSinceLastUse += Time.deltaTime;
            if (item == null || timeSinceLastUse <= item.Cooldown) return;

            if (!FocusedCell.HasValue) return;
            if (world.CellIntersects(FocusedCell.Value, playerCollider.bounds)) return;
            if (!Input.Actions.Player.Use.IsPressed()) return;

            timeSinceLastUse = 0;

            if (!item.CanUseOnBlock(world.GetBlock(FocusedCell.Value))) return;

            if (item is Pickaxe pickaxe)
            {
                BlockTile block = world.GetBlock(FocusedCell.Value);
                if (world.DamageBlock(FocusedCell.Value, pickaxe.Power))
                    inventory.AddItem(block, 1);
            }
            else if (item is BlockTile block)
            {
                if (world.PlaceBlock(FocusedCell.Value, block, inventory.FirstPickaxe.Power))
                    inventory.RemoveItem(block, 1);
            }
        }

        private void AssignCells()
        {
            Vector3 mouseWorld = input.MouseWorldPoint;
            MouseCell = world.WorldToCell(mouseWorld);

            Vector2 hotspot = (Vector2)transform.position + hotspotOffset;
            rangePath = Vector2.ClampMagnitude((Vector2)mouseWorld - hotspot, range);

            if (!smartCursor || inventory.HotbarSelected?.Item is not Pickaxe)
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

        private void PlayPlaceSound(Vector3Int cell, BlockTile block) => audioSource.PlayOneShot(block.placeSound);
        private void PlayHitSound(Vector3Int cell, BlockTile block) => audioSource.PlayOneShot(block.hitSound);

        private void OnDrawGizmosSelected()
        {
            if (!smartCursor) return;

            Vector2 hotspot = (Vector2)transform.position + hotspotOffset;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(hotspot, hotspot + rangePath);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hitPoint, .1f);
        }
    }
}
