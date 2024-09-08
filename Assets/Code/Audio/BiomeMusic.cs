using System.Collections;
using FMOD.Studio;
using FMODUnity;
using SaintsField;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Audio
{
    public class BiomeMusic : MonoBehaviour
    {
        [Header("FMOD Events")]
        [SerializeField] EventReference biomeMusicEvent;

        [Header("References")]
        [SerializeField, Required] EntityLocationDeterminer playerLocation;

        [Header("Config")]
        [SerializeField] Biome startingBiome;

        private EventInstance musicInstance;
        private PARAMETER_DESCRIPTION paramBiome;
        private PARAMETER_DESCRIPTION paramPlayerLocation;

        private IEnumerator Start()
        {
            while (!RuntimeManager.HaveAllBanksLoaded)
                yield return null;

            EventDescription musicDescription = RuntimeManager.GetEventDescription(biomeMusicEvent);
            musicDescription.getParameterDescriptionByName("Biome", out paramBiome);
            musicDescription.getParameterDescriptionByName("Player Location", out paramPlayerLocation);
            musicDescription.createInstance(out musicInstance);

            SetBiome(startingBiome);
            musicInstance.start();
        }

        private void Update() =>
            musicInstance.setParameterByID(paramPlayerLocation.id, playerLocation.Location.GetHashCode());

        private void SetBiome(Biome biome) =>
            musicInstance.setParameterByID(paramBiome.id, (float)biome);
    }
}
