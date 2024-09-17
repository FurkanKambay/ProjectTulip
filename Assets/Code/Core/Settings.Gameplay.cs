using System;
using Unity.Properties;
using UnityEngine;

namespace Tulip.Core
{
    public partial class Settings
    {
        [Serializable]
        public sealed record GameplaySettingsBag
        {
            [SerializeField] bool useSmartCursor = true;
            [SerializeField] bool allowPause = true;

            internal GameplaySettingsBag()
            {
            }

            [CreateProperty]
            public bool UseSmartCursor
            {
                get => useSmartCursor;
                set => UpdateSetting(ref useSmartCursor, value);
            }

            [CreateProperty]
            public bool AllowPause
            {
                get => allowPause;
                set => UpdateSetting(ref allowPause, value);
            }
        }
    }
}
