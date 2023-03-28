using System;
using Game.Data.Items;
using Game.Data.Tiles;
using UnityEngine;

namespace Game.Player
{
    public class WorldModifier : MonoBehaviour
    {
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
        private Vector3Int playerCell;

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
            playerCell = World.WorldToCell(transform.position);
            MouseCell = World.WorldToCell(Input.Instance.MouseWorldPoint);
            FocusedCell = Vector3Int.Distance(playerCell, MouseCell) <= range ? MouseCell : null;
        }

        private bool IntersectsPlayer(Vector3Int cell)
        {
            Bounds mouseCellBounds = World.CellBoundsWorld(cell);
            return playerCollider.bounds.Intersects(mouseCellBounds);
        }

        private void PlayPlaceSound(Vector3Int cell, BlockTile block) => audioSource.PlayOneShot(block.placeSound);
        private void PlayHitSound(Vector3Int cell, BlockTile block) => audioSource.PlayOneShot(block.hitSound);
    }
}
