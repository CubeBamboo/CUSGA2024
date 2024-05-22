using UnityEngine;

namespace CbUtils
{
    public class MonoSingletons<T> : MonoBehaviour where T : MonoSingletons<T>
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                bool isNew = !instance;
                if (!instance)
                {
                    var go = new GameObject("MonoSingleton:" + typeof(T).ToString()).AddComponent<T>();
                    Debug.Log($"New MonoSingleton Create: {go.name}");
                    instance.OnInstanceCall(isNew);
                }
                return instance;
            }

            private set => instance = value;
        }

        public static bool IsInstance => instance != null;

        protected void SetDontDestroyOnLoad()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void Awake()
        {
            if(IsInstance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
            OnAwake();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnInstanceCall(bool isNewObject) { }
    }
}
