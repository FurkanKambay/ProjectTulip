using UnityEngine;

namespace Tulip.Core
{
    public abstract class PersistentSingleton<T> : Singleton<T> where T : Component
    {
        protected override void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this as T;
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.NotEditable;
        }
    }
}
