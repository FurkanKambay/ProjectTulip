using Unity.Properties;

namespace Tulip.Helpers
{
    public static partial class Options
    {
        public sealed class GameOptions
        {
            private bool useSmartCursor;

            [CreateProperty]
            public bool UseSmartCursor
            {
                get => useSmartCursor;
                set => SetOption(Keys.GameSmartCursor, ref useSmartCursor, value);
            }

            internal GameOptions()
            {
            }

            private static class Keys
            {
                public const string GameSmartCursor = "game/smart-cursor";
            }
        }
    }
}
