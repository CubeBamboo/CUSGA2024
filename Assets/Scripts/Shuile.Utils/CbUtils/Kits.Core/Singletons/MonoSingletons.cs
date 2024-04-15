using System.Collections;
using System.Collections.Generic;
using System.Security;
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
                    new GameObject("MonoSingleton:" + typeof(T).ToString()).AddComponent<T>();
                instance.OnInstanceCall(isNew);
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
