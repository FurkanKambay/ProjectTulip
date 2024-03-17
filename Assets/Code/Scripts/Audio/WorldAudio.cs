using Tulip.Data;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Audio
{
    public class WorldAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] World world;
        [SerializeField] AudioSource audioSource;

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
