using UnityEngine;

namespace CbUtils.Unity
{
    public class MonoGlobalAccessor<T> : MonoBehaviour where T : MonoGlobalAccessor<T>
    {
        public static T Instance { get; private set; }

        private void Awake()
        {
            Instance = this as T;
            OnAwake();
        }
        protected virtual void OnAwake() { }
    }
}
