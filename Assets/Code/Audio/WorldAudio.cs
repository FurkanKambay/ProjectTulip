using FMODUnity;
using SaintsField;
using Tulip.Data;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Audio
{
    public class WorldAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] World world;
        [SerializeField, Required] StudioEventEmitter terraformSfx;

        private const string paramMaterial = "Material";
        private const string paramTerraformType = "Terraform Type";

        private void HandleTerraformed(TileModification modification)
        {
            terraformSfx.Play();
            terraformSfx.SetParameter(paramMaterial, (float)modification.Placeable.Material, ignoreseekspeed: true);
            terraformSfx.SetParameter(paramTerraformType, (float)modification.Kind, ignoreseekspeed: true);
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
    }
}
