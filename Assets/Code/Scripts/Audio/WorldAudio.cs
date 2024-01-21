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

        private void HandleTilePlaced(Vector3Int cell, WorldTile tile) => audioSource.PlayOneShot(tile.placeSound);
        private void HandleTileDestroyed(Vector3Int cell, WorldTile tile) => audioSource.PlayOneShot(tile.hitSound);

        private void OnEnable()
        {
            world.OnPlaceTile += HandleTilePlaced;
            world.OnHitTile += HandleTileDestroyed;
            world.OnDestroyTile += HandleTileDestroyed;
        }

        private void OnDisable()
        {
            world.OnPlaceTile -= HandleTilePlaced;
            world.OnHitTile -= HandleTileDestroyed;
            world.OnDestroyTile -= HandleTileDestroyed;
        }
    }
}
