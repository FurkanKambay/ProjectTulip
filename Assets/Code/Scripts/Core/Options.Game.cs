using Unity.Properties;

namespace Tulip.Core
{
    public static partial class Options
    {
        public sealed class GameOptions
        {
            private bool useSmartCursor = true;
            private bool allowPause = true;

            [CreateProperty]
            public bool UseSmartCursor
            {
                get => useSmartCursor;
                set => SetOption(Keys.SmartCursor, ref useSmartCursor, value);
            }

            [CreateProperty]
            public bool AllowPause
            {
                get => allowPause;
                set => SetOption(Keys.AllowPause, ref allowPause, value);
            }

            internal GameOptions()
            {
            }

            private static class Keys
            {
                public const string SmartCursor = "game/smart-cursor";
                public const string AllowPause = "game/allow-pause";
            }
        }
    }
}
