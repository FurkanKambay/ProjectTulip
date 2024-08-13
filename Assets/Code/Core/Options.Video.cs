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
            [SerializeField] FullScreenMode windowMode;
            [SerializeField] Resolution resolution;

            [CreateProperty]
            public string WindowMode
            {
                get => WindowModeToString(windowMode);
                set
                {
                    FullScreenMode newMode = StringToWindowMode(value);
                    Screen.fullScreenMode = newMode;
                    SetOption(Keys.FullScreenMode, ref windowMode, newMode);
                }
            }

            [CreateProperty]
            public string Resolution
            {
                get
                {
                    if (!IsResolutionValid(resolution))
                        resolution = StringToResolution(GetDefaultResolution());

                    return ResolutionToString(resolution);
                }
                set
                {
                    Resolution newResolution = StringToResolution(value);

                    if (!IsResolutionValid(newResolution))
                        return;

                    resolution = newResolution;
                    Screen.SetResolution(resolution.width, resolution.height, windowMode);
                    SetOption(Keys.Resolution, ref resolution, newResolution);
                }
            }

            [CreateProperty] List<string> supportedResolutions;

            [CreateProperty] List<string> supportedWindowModes = new()
            {
                WindowModeToString(FullScreenMode.FullScreenWindow),
                WindowModeToString(FullScreenMode.Windowed)
            };

            private VideoOptions()
            {
            }

            internal void LoadValues()
            {
                supportedResolutions = Screen.resolutions
                    .Select(ResolutionToString)
                    .Reverse()
                    .ToList();

                Resolution = LoadOption(Keys.Resolution, GetDefaultResolution());
                WindowMode = LoadOption(Keys.FullScreenMode, supportedWindowModes[0]);
            }

#region Converters

            private static string WindowModeToString(FullScreenMode value) =>
                windowModeLabels[value];

            private static FullScreenMode StringToWindowMode(string value) =>
                windowModeLabels.First(kvp => kvp.Value == value).Key;

            private static string ResolutionToString(Resolution value) =>
                $"{value.width}\u00d7{value.height}";

            private static Resolution StringToResolution(string value)
            {
#if UNITY_WEBGL
                return new Resolution();
#endif

                string[] resolutionParts = value.Split('\u00d7', 2);
                int width = int.Parse(resolutionParts[0]);
                int height = int.Parse(resolutionParts[1]);

                return new Resolution
                {
                    width = width,
                    height = height
                };
            }

#endregion

            private string GetDefaultResolution() =>
                supportedResolutions.Count > 0 ? supportedResolutions[0] : string.Empty;

            private bool IsResolutionValid(Resolution value) =>
                supportedResolutions.Contains(ResolutionToString(value));

            void ISerializationCallbackReceiver.OnBeforeSerialize() => resolution = StringToResolution(Resolution);
            void ISerializationCallbackReceiver.OnAfterDeserialize() { }

            // TODO: use Localization
            private static readonly Dictionary<FullScreenMode, string> windowModeLabels = new()
            {
                [FullScreenMode.ExclusiveFullScreen] = "Full Exclusive",
                [FullScreenMode.FullScreenWindow] = "Full Borderless",
                [FullScreenMode.MaximizedWindow] = "Maximized",
                [FullScreenMode.Windowed] = "Windowed"
            };

            internal static class Keys
            {
                public const string FullScreenMode = "video/full-screen-mode";
                public const string Resolution = "video/resolution";
            }
        }
    }
}
