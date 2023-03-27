using System;
using Game.Data.Items;
using Game.Data.Tiles;
using UnityEngine;

namespace Game.Player
{
    public class WorldModifier : MonoBehaviour
    {
        public bool smartCursor;

        public event Action<Vector3Int> CellFocusChanged;

        private Inventory inventory;
        private AudioSource audioSource;

        private float timeSinceLastUse;
        private Vector3Int focusedCell;

        private static World World => World.Instance;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
            audioSource = GetComponent<AudioSource>();

            World.BlockPlaced += PlayPlaceSound;
            World.BlockHit += PlayHitSound;
            World.BlockDestroyed += PlayHitSound;
        }

        private void Update()
        {
            IUsable item = inventory.HotbarSelected;
            Vector3Int cell = SetFocusedCell();

            timeSinceLastUse += Time.deltaTime;
            if (item == null || timeSinceLastUse <= item.UseTime) return;

            if (!Input.Actions.Player.Fire.IsPressed()) return;
            timeSinceLastUse = 0;

            if (item is Pickaxe)
                World.DamageBlock(cell, inventory.ActivePickaxe.power);
            else if (item is BlockTile block)
                World.PlaceBlock(cell, block, inventory.ActivePickaxe.power);
        }

        private Vector3Int SetFocusedCell()
        {
            Vector3Int cell = World.WorldToCell(Input.Instance.MouseWorldPoint);

            if (!smartCursor && cell != focusedCell)
            {
                focusedCell = cell;
                CellFocusChanged?.Invoke(cell);
            }

            return cell;
        }

        private void PlayPlaceSound(Vector3Int cell, BlockTile block) => audioSource.PlayOneShot(block.placeSound);
        private void PlayHitSound(Vector3Int cell, BlockTile block) => audioSource.PlayOneShot(block.hitSound);
    }
}
