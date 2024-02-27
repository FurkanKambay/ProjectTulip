using Unity.Properties;

namespace Tulip.Core
{
    public static partial class Options
    {
        public sealed class SoundOptions
        {
            private int masterVolume = 100;

            [CreateProperty]
            public int MasterVolume
            {
                get => masterVolume;
                set => SetOption(Keys.VolumeMaster, ref masterVolume, value);
            }

            private int musicVolume = 50;

            [CreateProperty]
            public int MusicVolume
            {
                get => musicVolume;
                set => SetOption(Keys.VolumeMusic, ref musicVolume, value);
            }

            private int effectsVolume = 100;

            [CreateProperty]
            public int EffectsVolume
            {
                get => effectsVolume;
                set => SetOption(Keys.VolumeEffects, ref effectsVolume, value);
            }

            private int uiVolume = 100;

            [CreateProperty]
            public int UIVolume
            {
                get => uiVolume;
                set => SetOption(Keys.VolumeUI, ref uiVolume, value);
            }

            internal SoundOptions()
            {
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
