using System;
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

            private List<string> supportedResolutions;

            public List<string> SupportedResolutions
            {
                get
                {
                    if (supportedResolutions is not { Count: > 0})
                    {
                        supportedResolutions = Screen.resolutions
                            .Select(r => $"{r.width}\u00d7{r.height}")
                            .Reverse()
                            .ToList();
                    }

                    return supportedResolutions;
                }
            }

            private VideoOptions() { }

            internal void LoadValues()
            {
                resolution = LoadOption(Keys.Resolution, GetDefaultResolution());
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
                get => IsResolutionValid(resolution) ? resolution : resolution = GetDefaultResolution();
                set
                {
                    if (!IsResolutionValid(value))
                        return;

                    string[] resolutionParts = Resolution.Split('\u00d7', 2);
                    int width = int.Parse(resolutionParts[0]);
                    int height = int.Parse(resolutionParts[1]);
                    Screen.SetResolution(width, height, FullScreenMode);

                    SetOption(Keys.Resolution, ref resolution, value);
                }
            }

            private string GetDefaultResolution() =>
                SupportedResolutions.Count > 0 ? SupportedResolutions[0] : string.Empty;

            private bool IsResolutionValid(string value) =>
                SupportedResolutions.Contains(value);

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
