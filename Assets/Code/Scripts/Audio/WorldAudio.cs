using Tulip.Data;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Audio
{
    public class WorldAudio : MonoBehaviour
    {
        private AudioSource audioSource;

        private void Awake() => audioSource = GetComponent<AudioSource>();

        private void HandleTilePlaced(TileModification modification)
            => audioSource.PlayOneShot(modification.WorldTile.placeSound);

        private void HandleTileHit(TileModification modification)
            => audioSource.PlayOneShot(modification.WorldTile.hitSound);

        private void HandleTileDestroyed(TileModification modification)
            => audioSource.PlayOneShot(modification.WorldTile.destroySound);

        private void OnEnable()
        {
            World.Instance.OnPlaceTile += HandleTilePlaced;
            World.Instance.OnHitTile += HandleTileHit;
            World.Instance.OnDestroyTile += HandleTileDestroyed;
        }

        private void OnDisable()
        {
            World.Instance.OnPlaceTile -= HandleTilePlaced;
            World.Instance.OnHitTile -= HandleTileHit;
            World.Instance.OnDestroyTile -= HandleTileDestroyed;
        }
    }
}
