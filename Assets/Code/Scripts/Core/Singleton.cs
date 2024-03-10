using UnityEngine;

namespace Tulip.Core
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T instance;

        public static T Instance
        {
            get
            {
                if (instance) return instance;

                instance = FindAnyObjectByType<T>();
                if (instance)
                {
                    Debug.LogWarning($"{typeof(T).Name} instance was null. Found an existing one.");
                    return instance;
                }

                Debug.LogWarning($"{typeof(T).Name} instance could not be found. Creating a new one.");
                var gameObject = new GameObject { name = typeof(T).Name, hideFlags = HideFlags.NotEditable };
                instance = gameObject.AddComponent<T>();

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning($"{typeof(T).Name} instance already exists. Destroying self.");
                Destroy(gameObject);
                return;
            }

            instance = this as T;
            gameObject.hideFlags = HideFlags.NotEditable;

            Debug.Log($"{typeof(T).Name} instance is ready.");
        }
    }
}
