using System;
using UnityEngine;

namespace Tulip.Core
{
    [CreateAssetMenu(fileName = "Game Options", menuName = "Data/Game Options")]
    public partial class Options : ScriptableObject
    {
        public static Options Instance { get; private set; }

        public static event Action OnUpdate;

        public GameOptions Gameplay => gameplay;
        public SoundOptions Sound => sound;
        public VideoOptions Video => video;

        [SerializeField] private GameOptions gameplay;
        [SerializeField] private SoundOptions sound;
        [SerializeField] private VideoOptions video;

        private void OnEnable()
        {
            Instance = this;
            gameplay.LoadValues();
            sound.LoadValues();
            video.LoadValues();
        }

        private static int LoadOption(string key, int defaultValue) => PlayerPrefs.GetInt(key, defaultValue);
        private static float LoadOption(string key, float defaultValue) => PlayerPrefs.GetFloat(key, defaultValue);
        private static string LoadOption(string key, string defaultValue) => PlayerPrefs.GetString(key, defaultValue);

        private static bool LoadOption(string key, bool defaultValue) =>
            PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;

        private static T LoadOption<T>(string key, T defaultValue) where T : Enum
            => (T)(object)PlayerPrefs.GetInt(key, defaultValue.GetHashCode());

        private static void SetOption<T>(string key, ref T field, T newValue)
        {
            if (field.Equals(newValue)) return;

            switch (newValue)
            {
                case Enum value:
                    PlayerPrefs.SetInt(key, value.GetHashCode());
                    break;
                case bool value:
                    PlayerPrefs.SetInt(key, value ? 1 : 0);
                    break;
                case int value:
                    PlayerPrefs.SetInt(key, value);
                    break;
                case float value:
                    PlayerPrefs.SetFloat(key, value);
                    break;
                case string value:
                    PlayerPrefs.SetString(key, value);
                    break;
            }

            field = newValue;
            OnUpdate?.Invoke();

            PlayerPrefs.Save();
        }
    }
}
