using System;
using System.Collections.Generic;

namespace Shuile.Core.Framework
{
    public interface ITypeEvent
    {
    }

    public class TypeEventSystem
    {
        public class Event<T> : ITypeEvent
        {
            public System.Action<T> action;
        }

        private class EventContainer
        {
            readonly Dictionary<Type, object> eventsContainer = new();
            public bool ContainsKey(Type type) => eventsContainer.ContainsKey(type);
            public Event<T> GetEvent<T>()
            {
                return eventsContainer.TryGetValue(typeof(T), out var ret) ? (Event<T>)ret : null;
            }
            public Event<T> GetOrAddEvent<T>()
            {
                var type = typeof(T);
                if (!eventsContainer.ContainsKey(type))
                {
                    Event<T> newAction = new();
                    eventsContainer.Add(type, newAction);
                }
                return (Event<T>)(eventsContainer[type]);
            }
            public void Remove(Type type) => eventsContainer.Remove(type);
        }

        private static readonly Lazy<TypeEventSystem> global = new(() => new TypeEventSystem());
        public static TypeEventSystem Global => global.Value;

        private readonly EventContainer eventsContainer = new();

        public void Register<T>(Action<T> action) where T : ITypeEvent
        {
            var res = eventsContainer.GetOrAddEvent<T>();
            res.action += action;
        }
        public void UnRegister<T>(Action<T> action) where T : ITypeEvent
        {
            var res = eventsContainer.GetEvent<T>();
            if(res != null)
                res.action -= action;
        }
        public void ClearEventOf<T>() where T : ITypeEvent => eventsContainer.Remove(typeof(T));
        public void Trigger<T>(T para) where T : ITypeEvent
        {
            var type = typeof(T);
            if (!eventsContainer.ContainsKey(type)) return;
            eventsContainer.GetEvent<T>().action?.Invoke(para);
        }
    }
}
