using System;
using UnityEngine;

namespace Tulip.Core
{
    [Serializable]
    public partial class Settings
    {
        public static event Action OnUpdate;

        [SerializeField] private GameplaySettingsBag gameplay;
        [SerializeField] private AudioSettingsBag audio;
        [SerializeField] private VideoSettingsBag video;

        public static Settings Instance { get; private set; } = new();

        // ReSharper disable UnusedMember.Global
        public static GameplaySettingsBag Gameplay => Instance.gameplay;
        public static AudioSettingsBag Audio => Instance.audio;
        public static VideoSettingsBag Video => Instance.video;
        // ReSharper restore UnusedMember.Global

        internal Settings()
        {
            gameplay = new GameplaySettingsBag();
            audio = new AudioSettingsBag();
            video = new VideoSettingsBag();
        }

        internal static void SetInstance(Settings value)
        {
            if (value == null)
                return;

            Instance = value;
            OnUpdate?.Invoke();
        }

        internal static void UpdateSetting<T>(ref T field, T newValue)
        {
            if (field.Equals(newValue))
                return;

            field = newValue;
            OnUpdate?.Invoke();
        }
    }
}
