using UnityEngine;

namespace Game.Helpers
{
    public abstract class PersistentSingleton<T> : Singleton<T> where T : Component
    {
        protected override void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
