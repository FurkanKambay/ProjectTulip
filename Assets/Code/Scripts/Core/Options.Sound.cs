using System;
using Unity.Properties;
using UnityEngine;

namespace Tulip.Core
{
    public partial class Options
    {
        [Serializable]
        public sealed record SoundOptions
        {
            [SerializeField, Range(0, 100)] int masterVolume;
            [SerializeField, Range(0, 100)] int musicVolume;
            [SerializeField, Range(0, 100)] int effectsVolume;
            [SerializeField, Range(0, 100)] int uiVolume;

            private SoundOptions() { }

            internal void LoadValues()
            {
                MasterVolume = LoadOption(Keys.VolumeMaster, 100);
                MusicVolume = LoadOption(Keys.VolumeMusic, 80);
                EffectsVolume = LoadOption(Keys.VolumeEffects, 100);
                UIVolume = LoadOption(Keys.VolumeUI, 100);
            }

            [CreateProperty]
            public int MasterVolume
            {
                get => masterVolume;
                set => SetOption(Keys.VolumeMaster, ref masterVolume, value);
            }

            [CreateProperty]
            public int MusicVolume
            {
                get => musicVolume;
                set => SetOption(Keys.VolumeMusic, ref musicVolume, value);
            }

            [CreateProperty]
            public int EffectsVolume
            {
                get => effectsVolume;
                set => SetOption(Keys.VolumeEffects, ref effectsVolume, value);
            }

            [CreateProperty]
            public int UIVolume
            {
                get => uiVolume;
                set => SetOption(Keys.VolumeUI, ref uiVolume, value);
            }

            internal static class Keys
            {
                public const string VolumeMaster = "sound/master";
                public const string VolumeMusic = "sound/music";
                public const string VolumeEffects = "sound/effects";
                public const string VolumeUI = "sound/ui";
            }
        }
    }
}
