using Tulip.Data;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Audio
{
    public class WorldAudio : MonoBehaviour
    {
        private World world;
        private AudioSource audioSource;

        private void Awake()
        {
            world = FindAnyObjectByType<World>();
            audioSource = GetComponent<AudioSource>();
        }

        private void HandleTilePlaced(TileModification modification)
            => audioSource.PlayOneShot(modification.WorldTile.placeSound);

        private void HandleTileHit(TileModification modification)
            => audioSource.PlayOneShot(modification.WorldTile.hitSound);

        private void HandleTileDestroyed(TileModification modification)
            => audioSource.PlayOneShot(modification.WorldTile.destroySound);

        private void OnEnable()
        {
            world.OnPlaceTile += HandleTilePlaced;
            world.OnHitTile += HandleTileHit;
            world.OnDestroyTile += HandleTileDestroyed;
        }

        private void OnDisable()
        {
            world.OnPlaceTile -= HandleTilePlaced;
            world.OnHitTile -= HandleTileHit;
            world.OnDestroyTile -= HandleTileDestroyed;
        }
    }
}
