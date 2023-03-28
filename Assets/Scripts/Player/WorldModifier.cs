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
            playerCell = World.WorldToCell(transform.position);

            timeSinceLastUse += Time.deltaTime;
            if (item == null || timeSinceLastUse <= item.UseTime) return;

            SetFocusedCell();
            if (!focusedCell.HasValue) return;
            if (IntersectsPlayer(focusedCell.Value)) return;
            if (!Input.Actions.Player.Fire.IsPressed()) return;

            timeSinceLastUse = 0;

            if (item is Pickaxe)
                World.DamageBlock(focusedCell.Value, inventory.ActivePickaxe.power);
            else if (item is BlockTile block)
                World.PlaceBlock(focusedCell.Value, block, inventory.ActivePickaxe.power);
        }

        private void SetFocusedCell()
        {
            Vector3Int mouseCell = World.WorldToCell(Input.Instance.MouseWorldPoint);

            if (smartCursor) return;

            if (Vector3Int.Distance(playerCell, mouseCell) > range)
            {
                focusedCell = null;
                CellFocusChanged?.Invoke(focusedCell);
                return;
            }

            if (focusedCell == mouseCell) return;
            focusedCell = mouseCell;
            CellFocusChanged?.Invoke(focusedCell);
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
