using UnityEngine;

namespace CbUtils
{
    /// <summary> singletons that can only spawn by your hand </summary>
    public abstract class MonoNotAutoSpawnSingletons<T> : MonoBehaviour where T : MonoNotAutoSpawnSingletons<T>
    {
        public static T Instance { get; protected set; }

        protected virtual void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
            OnAwake();
        }

        public static void SpawnNew()
        {
            new GameObject("MonoSingleton:" + typeof(T)).AddComponent<T>();
        }

        protected virtual void OnAwake() { }
    }
}
