using System;
using Game.Data.Items;
using Game.Data.Tiles;
using UnityEngine;

namespace Game.Player
{
    public class WorldModifier : MonoBehaviour
    {
        [SerializeField] private Vector2 hotspotOffset;
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
        private Vector3 hitpoint;

        private static World World => World.Instance;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
            audioSource = GetComponent<AudioSource>();
            playerCollider = GetComponent<BoxCollider2D>();

            World.BlockPlaced += PlayPlaceSound;
            World.BlockHit += PlayHitSound;
            World.BlockDestroyed += PlayHitSound;
        }

        private void Update()
        {
            IUsable item = inventory.HotbarSelected;
            AssignCells();

            timeSinceLastUse += Time.deltaTime;
            if (item == null || timeSinceLastUse <= item.UseTime) return;

            if (!FocusedCell.HasValue) return;
            if (IntersectsPlayer(FocusedCell.Value)) return;
            if (!Input.Actions.Player.Fire.IsPressed()) return;

            timeSinceLastUse = 0;

            if (!item.CanUseOnBlock(World.GetBlock(FocusedCell.Value))) return;

            if (item is Pickaxe)
                World.DamageBlock(FocusedCell.Value, inventory.ActivePickaxe.power);
            else if (item is BlockTile block)
                World.PlaceBlock(FocusedCell.Value, block, inventory.ActivePickaxe.power);
        }

        private void AssignCells()
        {
            Vector3 mouseWorld = Input.Instance.MouseWorldPoint;
            MouseCell = World.WorldToCell(mouseWorld);

            Vector2 hotspot = (Vector2)transform.position + hotspotOffset;
            rangePath = Vector2.ClampMagnitude((Vector2)mouseWorld - hotspot, range);

            if (!smartCursor || inventory.HotbarSelected is not Pickaxe)
            {
                float distance = Vector3.Distance(hotspot, mouseWorld);
                FocusedCell = distance <= range ? MouseCell : null;
                return;
            }

            RaycastHit2D hit = Physics2D.Raycast(
                hotspot, rangePath, range,
                LayerMask.GetMask("World"));

            hitpoint = hit.point - (hit.normal * 0.1f);
            FocusedCell = hit.collider ? World.WorldToCell(hitpoint) : null;
        }

        private bool IntersectsPlayer(Vector3Int cell)
            => playerCollider.bounds.Intersects(World.CellBoundsWorld(cell));

        private void PlayPlaceSound(Vector3Int cell, BlockTile block) => audioSource.PlayOneShot(block.placeSound);
        private void PlayHitSound(Vector3Int cell, BlockTile block) => audioSource.PlayOneShot(block.hitSound);

        private void OnDrawGizmosSelected()
        {
            Vector2 hotspot = (Vector2)transform.position + hotspotOffset;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(hotspot,  hotspot + rangePath);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hitpoint, .1f);
        }
    }
}
