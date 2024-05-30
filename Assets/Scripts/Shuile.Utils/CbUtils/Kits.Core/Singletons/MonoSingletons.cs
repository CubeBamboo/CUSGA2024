using UnityEngine;

namespace CbUtils.Unity
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
                    SingletonCreator.GetMonoBehaviourSingletons<T>();
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

        public static void TryAccessInstance(System.Action<T> action)
        {
            if (IsInstance)
            {
                action(instance);
            }
        }
    }
}
