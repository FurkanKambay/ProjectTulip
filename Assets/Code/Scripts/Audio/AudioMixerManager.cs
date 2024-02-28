using Tulip.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace Tulip.Audio
{
    public class AudioMixerManager : MonoBehaviour
    {
        [SerializeField] AudioMixer mixer;

        private void Start() => HandleOptionsUpdated();

        private void HandleOptionsUpdated()
        {
            SetMixerValue("Master", Options.Sound.MasterVolume);
            SetMixerValue("Music", Options.Sound.MusicVolume);
            SetMixerValue("Effects", Options.Sound.EffectsVolume);
            SetMixerValue("UI", Options.Sound.UIVolume);
        }

        private void SetMixerValue(string channel, float value)
        {
            float mixerValue = Mathf.Max(0.0001f, Mathf.InverseLerp(0f, 100f, value));
            mixer.SetFloat(channel, Mathf.Log10(mixerValue) * 20f);
        }

        private void OnEnable() => Options.OnUpdate += HandleOptionsUpdated;
        private void OnDisable() => Options.OnUpdate -= HandleOptionsUpdated;
    }
}
