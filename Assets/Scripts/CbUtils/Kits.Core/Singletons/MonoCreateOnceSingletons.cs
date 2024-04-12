using UnityEngine;

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
                if (!instance && !isDestroy)
                    new GameObject("MonoSingleton:" + typeof(T).ToString()).AddComponent<T>();
                instance.OnInstanceCall(isNew);
                return instance;
            }

            private set => instance = value;
        }

        public static bool IsInstance => instance != null;
        private static bool isDestroy = false;

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

            isDestroy = false;
            Instance = this as T;

            OnAwake();
        }
        private void OnDestroy()
        {
            isDestroy = true;
        }

        protected virtual void OnAwake() { }
        protected virtual void OnInstanceCall(bool isNewObject) { }
    }
}
