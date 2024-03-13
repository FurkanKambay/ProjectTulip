﻿using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;

namespace Tulip.Core
{
    public partial class Options
    {
        [Serializable]
        public sealed record VideoOptions : ISerializationCallbackReceiver
        {
            [SerializeField] FullScreenMode fullScreenMode;
            [SerializeField] string resolution;

            public List<string> SupportedResolutions { get; private set; }

            private VideoOptions() { }

            internal void LoadValues()
            {
                SupportedResolutions = Screen.resolutions.Select(r => $"{r.width}\u00d7{r.height}").Reverse().ToList();
                resolution = LoadOption(Keys.Resolution, SupportedResolutions[0]);
                FullScreenMode = LoadOption(Keys.FullScreenMode, FullScreenMode.FullScreenWindow);
            }

            [CreateProperty]
            public FullScreenMode FullScreenMode
            {
                get => fullScreenMode;
                set => SetOption(Keys.FullScreenMode, ref fullScreenMode, value);
            }

            [CreateProperty]
            public string Resolution
            {
                get => IsResolutionValid(resolution) ? resolution : Resolution = SupportedResolutions[0];
                set
                {
                    if (!IsResolutionValid(value)) return;
                    SetOption(Keys.Resolution, ref resolution, value);
                }
            }

            private bool IsResolutionValid(string value) => SupportedResolutions.Contains(value);

            void ISerializationCallbackReceiver.OnBeforeSerialize() => resolution = Resolution;
            void ISerializationCallbackReceiver.OnAfterDeserialize() { }

            internal static class Keys
            {
                public const string FullScreenMode = "video/full-screen-mode";
                public const string Resolution = "video/resolution";
            }
        }
    }
}
