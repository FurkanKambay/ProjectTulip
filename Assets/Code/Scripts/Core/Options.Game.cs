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
            [SerializeField] bool useSmartCursor;
            [SerializeField] bool allowPause;

            private GameOptions() { }

            internal void LoadValues()
            {
                UseSmartCursor = LoadOption(Keys.SmartCursor, false);
                AllowPause = LoadOption(Keys.SmartCursor, false);
            }

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

            internal static class Keys
            {
                public const string SmartCursor = "game/smart-cursor";
                public const string AllowPause = "game/allow-pause";
            }
        }
    }
}
