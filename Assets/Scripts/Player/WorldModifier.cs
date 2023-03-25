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

            World.Instance.BlockPlaced += PlayPlaceSound;
            World.Instance.BlockHit += PlayHitSound;
            World.Instance.BlockDestroyed += PlayHitSound;
        }

        private void Update()
        {
            IUsable item = inventory.HotbarSelected;
            Vector3Int cell = World.Instance.WorldToCell(Input.Instance.MouseWorldPoint);

            timeSinceLastUse += Time.deltaTime;
            if (item == null || timeSinceLastUse <= item.UseTime) return;

            if (!Input.Actions.Player.Fire.IsPressed()) return;

            timeSinceLastUse = 0;
            item.Use(cell, inventory.ActivePickaxe);
        }

        private void PlayPlaceSound(Vector3Int cell, BlockTile block) => audioSource.PlayOneShot(block.placeSound);
        private void PlayHitSound(Vector3Int cell, BlockTile block) => audioSource.PlayOneShot(block.hitSound);
    }
}
