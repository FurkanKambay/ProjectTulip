using UnityEngine;

namespace Game.Helpers
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T instance;

        public static T Instance =>
            instance ??= FindObjectOfType<T>() ?? new GameObject
            {
                name = typeof(T).Name,
                hideFlags = HideFlags.NotEditable
            }.AddComponent<T>();

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this as T;
            gameObject.hideFlags = HideFlags.NotEditable;
        }
    }
}