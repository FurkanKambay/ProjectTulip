using System;
using UnityEngine;

namespace Tulip.Helpers
{
    public static partial class Options
    {
        public static readonly GameOptions Game = new();
        public static readonly SoundOptions Sound = new();

        public static event Action OnUpdate;

        private static void SetOption<T>(string key, ref T field, T newValue)
        {
            if (field.Equals(newValue)) return;

            switch (newValue)
            {
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
