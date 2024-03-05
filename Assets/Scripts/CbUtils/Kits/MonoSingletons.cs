using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CbUtils
{
    public class MonoSingletons<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                //lazy load
                if (!instance && !isQuit)
                {
                    var go = new GameObject("MonoSingleton:" + typeof(T).ToString());
                    instance = go.AddComponent<T>();
                }

                return instance;
            }

            private set
            {
                instance = value;
            }
        }

        public static bool IsInstance => instance != null;
        private static bool isQuit; //to prevent someone access singletons on application quit.

        private void OnApplicationQuit()
        {
            isQuit = true;
        }

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

            isQuit = false;
            Instance = this as T;
        }
    }
}
