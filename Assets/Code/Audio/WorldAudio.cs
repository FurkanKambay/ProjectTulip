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
            terraformSfx.SetParameter(paramMaterial, (float)modification.Placeable.Material);
            terraformSfx.SetParameter(paramTerraformType, (float)modification.Kind);
            terraformSfx.Play();
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
