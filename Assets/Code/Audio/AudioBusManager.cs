using System.Collections;
using FMOD.Studio;
using FMODUnity;
using Tulip.Core;
using UnityEngine;

namespace Tulip.Audio
{
    public class AudioBusManager : MonoBehaviour
    {
        private Bus masterBus;
        private Bus musicBus;
        private Bus sfxBus;
        private Bus uiBus;

        private void Awake() => StartCoroutine(LoadBuses());
        private void Start() => HandleOptionsUpdated();

        private void OnEnable() => Options.OnUpdate += HandleOptionsUpdated;
        private void OnDisable() => Options.OnUpdate -= HandleOptionsUpdated;

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

        private IEnumerator LoadBuses()
        {
            while (!RuntimeManager.HaveAllBanksLoaded)
                yield return null;

            masterBus = RuntimeManager.GetBus("bus:/");
            musicBus = RuntimeManager.GetBus("bus:/Music");
            sfxBus = RuntimeManager.GetBus("bus:/SFX");
            uiBus = RuntimeManager.GetBus("bus:/UI");
        }

        private void HandleOptionsUpdated()
        {
            SetVolume(masterBus, Options.Instance.Sound.MasterVolume);
            SetVolume(musicBus, Options.Instance.Sound.MusicVolume);
            SetVolume(sfxBus, Options.Instance.Sound.EffectsVolume);
            SetVolume(uiBus, Options.Instance.Sound.UIVolume);
        }

        private static void SetVolume(Bus bus, int value) => bus.setVolume(Mathf.InverseLerp(0, 100, value));
    }
}
