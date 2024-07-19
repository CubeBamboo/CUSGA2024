using System;
using System.Collections.Generic;

namespace CbUtils
{
    /// <summary>
    ///     a global event system use string
    /// </summary>
    public class StringEventManager : CSharpLazySingletons<StringEventManager>
    {
        private readonly Dictionary<string, Action> eventDict = new();

        public void Register(string eventName, Action handler)
        {
            if (eventDict.ContainsKey(eventName))
            {
                eventDict[eventName] += handler;
            }
            else
            {
                eventDict.Add(eventName, handler);
            }
        }

        public void UnRegister(string eventName, Action handler)
        {
            if (eventDict.ContainsKey(eventName))
            {
                eventDict[eventName] -= handler;
            }
        }

        public void Trigger(string eventName)
        {
            if (eventDict.ContainsKey(eventName))
            {
                eventDict[eventName]?.Invoke();
            }
        }

        public void Clear()
        {
            eventDict.Clear();
        }
    }
}
