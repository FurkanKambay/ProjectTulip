using UnityEngine;

namespace Tulip.Core
{
    public abstract class PersistentSingleton<T> : Singleton<T> where T : Component
    {
        protected override void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning($"{typeof(T).Name} instance already exists. Destroying self.");
                Destroy(gameObject);
                return;
            }

            instance = this as T;
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.NotEditable;

            Debug.Log($"{typeof(T).Name} instance is ready.");
        }
    }
}
