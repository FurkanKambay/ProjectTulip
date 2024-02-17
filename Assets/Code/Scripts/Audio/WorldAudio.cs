using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Audio
{
    public class WorldAudio : MonoBehaviour
    {
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void HandleTilePlaced(Vector3Int cell, WorldTile tile) => audioSource.PlayOneShot(tile.placeSound);
        private void HandleTileHit(Vector3Int cell, WorldTile tile) => audioSource.PlayOneShot(tile.hitSound);
        private void HandleTileDestroyed(Vector3Int cell, WorldTile tile) => audioSource.PlayOneShot(tile.destroySound);

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
