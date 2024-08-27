using System.Collections;
using FMOD.Studio;
using FMODUnity;
using Tulip.Data;
using Tulip.Data.Items;
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

        private PARAMETER_DESCRIPTION paramMaterial;
        private PARAMETER_DESCRIPTION paramDamage;

        private IEnumerator Start()
        {
            while (!RuntimeManager.HaveAllBanksLoaded)
                yield return null;

            EventDescription description = RuntimeManager.GetEventDescription(tileDamagedEvent);
            description.getParameterDescriptionByName("Material", out paramMaterial);
            description.getParameterDescriptionByName("Damage", out paramDamage);
        }

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

            PlaceableData placeableData = modification.PlaceableData;

            if (modification.Kind == TileModificationKind.Damaged)
            {
                int tileDamage = world.GetTileDamage(modification.Cell, placeableData.TileType);
                float tileHealth = (float)tileDamage / placeableData.Hardness;
                sfx.setParameterByID(paramDamage.id, tileHealth);
            }

            sfx.set3DAttributes(world.CellCenter(modification.Cell).To3DAttributes());
            sfx.setParameterByID(paramMaterial.id, (float)placeableData.Material, ignoreseekspeed: true);

            sfx.start();
            sfx.release();
        }
    }
}
