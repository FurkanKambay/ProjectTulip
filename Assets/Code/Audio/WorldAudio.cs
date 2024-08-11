using FMOD.Studio;
using FMODUnity;
using Tulip.Data;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Audio
{
    public class WorldAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] World world;

        [Header("FMOD Events")]
        [SerializeField] EventReference tilePlacedEvent;
        [SerializeField] EventReference tileDamagedEvent;
        [SerializeField] EventReference tileDestroyedEvent;

        private const string paramMaterial = "Material";
        private const string paramTerraformType = "Terraform Type";

        private void OnEnable()
        {
            world.OnPlaceTile += HandleTerraformed;
            world.OnHitTile += HandleTerraformed;
            world.OnDestroyTile += HandleTerraformed;
        }

        private void OnDisable()
        {
            world.OnPlaceTile -= HandleTerraformed;
            world.OnHitTile -= HandleTerraformed;
            world.OnDestroyTile -= HandleTerraformed;
        }

        private void HandleTerraformed(TileModification modification)
        {
            EventInstance sfx = modification.Kind switch
            {
                TileModificationKind.Placed => RuntimeManager.CreateInstance(tilePlacedEvent),
                TileModificationKind.Damaged => RuntimeManager.CreateInstance(tileDamagedEvent),
                _ => RuntimeManager.CreateInstance(tileDestroyedEvent)
            };

            sfx.set3DAttributes(world.CellCenter(modification.Cell).To3DAttributes());
            sfx.setParameterByName(paramMaterial, (float)modification.Placeable.Material, ignoreseekspeed: true);
            sfx.setParameterByName(paramTerraformType, (float)modification.Kind, ignoreseekspeed: true);

            sfx.start();
            sfx.release();
        }
    }
}
