using UnityEngine;

namespace Game.Helpers
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;

        public static T Instance => instance
            ? instance
            : instance
                = FindObjectOfType<T>() ?? new GameObject { name = typeof(T).Name }.AddComponent<T>();

        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}
