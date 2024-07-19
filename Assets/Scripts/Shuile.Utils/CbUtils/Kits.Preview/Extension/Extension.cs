using CbUtils.Extension;
using CbUtils.Preview.Event;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace CbUtils.Preview.Extension
{
    public static class Extension
    {
        public static void SetOnUpdate(this GameObject gameObject, Action action)
        {
            gameObject.GetOrAddComponent<UpdateEventMono>().OnUpdate += action;
        }

        #region UnityEngine.Events

        public static ICustomUnRegister AddListenerWithCustomUnRegister(this UnityEvent self, UnityAction action)
        {
            self.AddListener(action);
            return new CustomUnRegister(() => self.RemoveListener(action));
        }

        #endregion
    }
}
