using UnityEngine;

namespace Game.Helpers
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T instance;

        public static T Instance =>
            instance ??= FindObjectOfType<T>() ?? new GameObject { name = typeof(T).Name }.AddComponent<T>();

        protected virtual void Awake()
        {
            if (instance == null)
                instance = this as T;
            else
                Destroy(gameObject);
        }
    }
}
