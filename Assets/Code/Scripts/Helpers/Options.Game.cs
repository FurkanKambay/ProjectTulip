using Unity.Properties;

namespace Tulip.Helpers
{
    public static partial class Options
    {
        public sealed class GameOptions
        {
            private bool useSmartCursor = true;

            [CreateProperty]
            public bool UseSmartCursor
            {
                get => useSmartCursor;
                set => SetOption(Keys.SmartCursor, ref useSmartCursor, value);
            }

            internal GameOptions()
            {
            }

            private static class Keys
            {
                public const string SmartCursor = "game/smart-cursor";
            }
        }
    }
}
