using CbUtils.Extension;
using UnityEngine;
using CbUtils.Preview.Event;

namespace CbUtils.Preview.Extension
{
    public static class Extension
    {
        public static void SetOnUpdate(this GameObject gameObject, System.Action action)
        {
            gameObject.GetOrAddComponent<UpdateEventMono>().OnUpdate += action;
        }

        #region UnityEngine.Events

        public static ICustomUnRegister AddListenerWithCustomUnRegister(this UnityEngine.Events.UnityEvent self, UnityEngine.Events.UnityAction action)
        {
            self.AddListener(action);
            return new CustomUnRegister(() => self.RemoveListener(action));
        }

        #endregion
    }
}
