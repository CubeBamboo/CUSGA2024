using System;
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
                var isNew = !instance;
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

        protected virtual void Awake()
        {
            if (IsInstance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
            OnAwake();
        }

        protected void SetDontDestroyOnLoad()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnAwake() { }
        protected virtual void OnInstanceCall(bool isNewObject) { }

        public static void TryAccessInstance(Action<T> action)
        {
            if (IsInstance)
            {
                action(instance);
            }
        }
    }
}
