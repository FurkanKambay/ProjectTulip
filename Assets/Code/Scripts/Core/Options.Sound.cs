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
            [SerializeField] int masterVolume = 100;
            [SerializeField] int musicVolume = 50;
            [SerializeField] int effectsVolume = 100;
            [SerializeField] int uiVolume = 100;

            private SoundOptions() { }

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

            private static class Keys
            {
                public const string VolumeMaster = "sound/master";
                public const string VolumeMusic = "sound/music";
                public const string VolumeEffects = "sound/effects";
                public const string VolumeUI = "sound/ui";
            }
        }
    }
}
