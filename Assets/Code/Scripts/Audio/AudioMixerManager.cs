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
            Set("Master", Convert(Options.Sound.MasterVolume));
            Set("Music", Convert(Options.Sound.MusicVolume));
            Set("Effects", Convert(Options.Sound.EffectsVolume));
            Set("UI", Convert(Options.Sound.UIVolume));
        }

        private static float Convert(int value)
            => Mathf.Max(0.0001f, Mathf.InverseLerp(0f, 100f, value));

        private void Set(string channel, float value)
            => mixer.SetFloat(channel, Mathf.Log10(value) * 20f);

        private void OnEnable() => Options.OnUpdate += HandleOptionsUpdated;
        private void OnDisable() => Options.OnUpdate -= HandleOptionsUpdated;
    }
}
