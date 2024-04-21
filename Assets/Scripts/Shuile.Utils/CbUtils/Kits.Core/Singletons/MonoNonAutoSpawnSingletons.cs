using UnityEngine;

namespace CbUtils
{
    /// <summary> singletons that can only spawn by your hand </summary>
    public abstract class MonoNonAutoSpawnSingletons<T> : MonoBehaviour where T : MonoNonAutoSpawnSingletons<T>
    {
        public static T Instance { get; protected set; }

        public static void SpawnNew()
        {
            new GameObject("MonoSingleton:" + typeof(T).ToString()).AddComponent<T>();
        }

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

        protected virtual void OnAwake() { }
    }
}
