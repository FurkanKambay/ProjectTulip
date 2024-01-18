using Game.Data.Tiles;
using UnityEngine;

namespace Game.Audio
{
    public class WorldAudio : MonoBehaviour
    {
        private AudioSource audioSource;
        private World world;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            world = World.Instance;
        }

        private void HandleBlockPlaced(Vector3Int cell, BlockTile block) => audioSource.PlayOneShot(block.placeSound);
        private void HandleBlockDestroyed(Vector3Int cell, BlockTile block) => audioSource.PlayOneShot(block.hitSound);

        private void OnEnable()
        {
            world.OnPlaceBlock += HandleBlockPlaced;
            world.OnHitBlock += HandleBlockDestroyed;
            world.OnDestroyBlock += HandleBlockDestroyed;
        }

        private void OnDisable()
        {
            world.OnPlaceBlock -= HandleBlockPlaced;
            world.OnHitBlock -= HandleBlockDestroyed;
            world.OnDestroyBlock -= HandleBlockDestroyed;
        }
    }
}
