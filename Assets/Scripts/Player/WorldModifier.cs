using Game.Data.Items;
using Game.Data.Tiles;
using UnityEngine;

namespace Game.Player
{
    public class WorldModifier : MonoBehaviour
    {
        private Inventory inventory;
        private AudioSource audioSource;

        private float timeSinceLastUse;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            IUsable item = inventory.HotbarSelected;
            if (item == null) return;

            Vector3Int cell = World.Instance.Tilemap.WorldToCell(Input.Instance.MouseWorldPoint);
            BlockTile block = World.Instance.Tilemap.GetTile<BlockTile>(cell);

            timeSinceLastUse += Time.deltaTime;
            if (timeSinceLastUse <= item.UseTime) return;

            if (!Input.Actions.Player.Fire.IsPressed()) return;

            if (block)
                audioSource.PlayOneShot(block.hitSound);
            else if (item is BlockTile tile)
                audioSource.PlayOneShot(tile.placeSound);

            timeSinceLastUse = 0;

            item.Use(cell, inventory.ActivePickaxe);
        }
    }
}
