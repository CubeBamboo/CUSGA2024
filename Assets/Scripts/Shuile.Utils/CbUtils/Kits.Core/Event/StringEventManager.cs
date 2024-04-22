using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CbUtils
{
    /// <summary>
    /// a global event system use string
    /// </summary>
    public class StringEventManager : CbUtils.MonoSingletons<StringEventManager>
    {
        protected override void Awake()
        {
            base.Awake();
            SetDontDestroyOnLoad();
        }

        #region EventManager

        private Dictionary<string, System.Action> eventDict = new Dictionary<string, System.Action>();

        public void Register(string eventName, System.Action handler)
        {
            if (eventDict.ContainsKey(eventName))
                eventDict[eventName] += handler;
            else
                eventDict.Add(eventName, handler);
        }
        public void UnRegister(string eventName, System.Action handler)
        {
            if (eventDict.ContainsKey(eventName))
                eventDict[eventName] -= handler;
        }
        public void Trigger(string eventName)
        {
            if (eventDict.ContainsKey(eventName))
                eventDict[eventName]?.Invoke();
        }

        public void Clear()
        {
            eventDict.Clear();
        }

        #endregion

    }
}
