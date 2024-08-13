using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Tulip.Audio
{
    public class BiomeMusic : MonoBehaviour
    {
        [Header("FMOD Events")]
        [SerializeField] EventReference biomeMusicEvent;

        [Header("Config")]
        [SerializeField] Biome startingBiome;

        private EventInstance musicInstance;
        private PARAMETER_DESCRIPTION paramBiome;

        private IEnumerator Start()
        {
            while (!RuntimeManager.HaveAllBanksLoaded)
                yield return null;

            EventDescription musicDescription = RuntimeManager.GetEventDescription(biomeMusicEvent);
            musicDescription.getParameterDescriptionByName("Biome", out paramBiome);
            musicDescription.createInstance(out musicInstance);

            SetBiome(startingBiome);
            musicInstance.start();
        }

        private void SetBiome(Biome biome) =>
            musicInstance.setParameterByID(paramBiome.id, (float)biome);
    }
}
