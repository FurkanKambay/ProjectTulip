using Unity.Properties;

namespace Tulip.Helpers
{
    public static partial class Options
    {
        public sealed class SoundOptions
        {
            private int masterVolume;

            [CreateProperty]
            public int MasterVolume
            {
                get => masterVolume;
                set => SetOption(Keys.VolumeMaster, ref masterVolume, value);
            }

            private int musicVolume;

            [CreateProperty]
            public int MusicVolume
            {
                get => musicVolume;
                set => SetOption(Keys.VolumeMusic, ref musicVolume, value);
            }

            private int effectsVolume;

            [CreateProperty]
            public int EffectsVolume
            {
                get => effectsVolume;
                set => SetOption(Keys.VolumeEffects, ref effectsVolume, value);
            }

            private int uiVolume;

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
