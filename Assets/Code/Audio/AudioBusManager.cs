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

        private void Awake()
        {
            masterBus = RuntimeManager.GetBus("bus:/");
            musicBus = RuntimeManager.GetBus("bus:/Music");
            sfxBus = RuntimeManager.GetBus("bus:/SFX");
            uiBus = RuntimeManager.GetBus("bus:/UI");
        }

        private void Start() => HandleOptionsUpdated();

        private void OnEnable() => Options.OnUpdate += HandleOptionsUpdated;
        private void OnDisable() => Options.OnUpdate -= HandleOptionsUpdated;

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
