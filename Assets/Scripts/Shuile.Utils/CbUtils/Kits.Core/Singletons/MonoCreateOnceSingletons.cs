using UnityEngine;
using UDebug = UnityEngine.Debug;

namespace CbUtils
{
    public class MonoCreateOnceSingletons<T> : MonoBehaviour where T : MonoCreateOnceSingletons<T>
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                bool isNew = !instance;
                if (!instance && CreateCount <= 0)
                    new GameObject("MonoSingleton:" + typeof(T).ToString()).AddComponent<T>();
                instance.OnInstanceCall(isNew);
                return instance;
            }

            private set => instance = value;
        }

        public static bool IsInstance => instance != null;
        public static int CreateCount { get; protected set; } = 0;

#if UNITY_EDITOR
        public static void ResetCreateCount() => CreateCount = 0;
#endif


        protected void SetDontDestroyOnLoad()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void Awake()
        {
            if (IsInstance)
            {
                Destroy(gameObject);
                return;
            }

            CreateCount++;
            Instance = this as T;

            OnAwake();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnInstanceCall(bool isNewObject) { }
    }
}
