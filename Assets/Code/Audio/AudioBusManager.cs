using System.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Settings = Tulip.Core.Settings;

namespace Tulip.Audio
{
    public class AudioBusManager : MonoBehaviour
    {
        private Bus masterBus;
        private Bus musicBus;
        private Bus sfxBus;
        private Bus uiBus;

        private async void Awake()
        {
            await WaitForAllBanksToLoad();

            masterBus = RuntimeManager.GetBus("bus:/");
            musicBus = RuntimeManager.GetBus("bus:/Music");
            sfxBus = RuntimeManager.GetBus("bus:/SFX");
            uiBus = RuntimeManager.GetBus("bus:/UI");
        }

        private void OnEnable() => Settings.OnUpdate += Settings_Updated;
        private void OnDisable() => Settings.OnUpdate -= Settings_Updated;

        private async void Start() => await UpdateVolumes();

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!RuntimeManager.StudioSystem.isValid())
                return;

            RuntimeManager.PauseAllEvents(!hasFocus);

            if (!hasFocus)
                RuntimeManager.CoreSystem.mixerSuspend();
            else
                RuntimeManager.CoreSystem.mixerResume();
        }

        internal static async Awaitable WaitForAllBanksToLoad()
        {
            while (!RuntimeManager.HaveAllBanksLoaded)
                await Awaitable.NextFrameAsync();
        }

        private async void Settings_Updated() => await UpdateVolumes();

        private async Task UpdateVolumes()
        {
            await WaitForAllBanksToLoad();

            SetVolume(masterBus, Settings.Audio.MasterVolume);
            SetVolume(musicBus, Settings.Audio.MusicVolume);
            SetVolume(sfxBus, Settings.Audio.EffectsVolume);
            SetVolume(uiBus, Settings.Audio.UIVolume);
        }

        private static void SetVolume(Bus bus, int value) =>
            bus.setVolume(Mathf.InverseLerp(0, 100, value));
    }
}
