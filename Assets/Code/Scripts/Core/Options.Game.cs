using System;
using Unity.Properties;
using UnityEngine;

namespace Tulip.Core
{
    public partial class Options
    {
        [Serializable]
        public sealed record GameOptions
        {
            [SerializeField] bool useSmartCursor = true;
            [SerializeField] bool allowPause = true;

            private GameOptions() { }

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

            private static class Keys
            {
                public const string SmartCursor = "game/smart-cursor";
                public const string AllowPause = "game/allow-pause";
            }
        }
    }
}
