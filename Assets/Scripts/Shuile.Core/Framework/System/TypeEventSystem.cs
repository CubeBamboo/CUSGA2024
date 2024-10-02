using System;
using System.Collections.Generic;

namespace Shuile.Core.Framework
{
    public interface ITypeEvent
    {
    }

    /// <summary>
    /// event id based on type, which can easily stored parameter
    /// </summary>
    public class TypeEventSystem
    {
        private static readonly Lazy<TypeEventSystem> global = new(() => new TypeEventSystem());

        private readonly EventContainer _eventsContainer = new();
        public static TypeEventSystem Global => global.Value;

        public void Register<T>(Action<T> action) where T : ITypeEvent
        {
            var res = _eventsContainer.GetOrAddEvent<T>();
            res.action += action;
        }

        public void UnRegister<T>(Action<T> action) where T : ITypeEvent
        {
            var res = _eventsContainer.GetEvent<T>();
            if (res != null)
            {
                res.action -= action;
            }
        }

        public void ClearEventOf<T>() where T : ITypeEvent
        {
            _eventsContainer.Remove(typeof(T));
        }

        public void Trigger<T>(T para) where T : ITypeEvent
        {
            var type = typeof(T);
            if (!_eventsContainer.ContainsKey(type))
            {
                return;
            }

            _eventsContainer.GetEvent<T>().action?.Invoke(para);
        }

        public class Event<T> : ITypeEvent
        {
            public Action<T> action;
        }

        private class EventContainer
        {
            private readonly Dictionary<Type, object> _eventsContainer = new();

            public bool ContainsKey(Type type)
            {
                return _eventsContainer.ContainsKey(type);
            }

            public Event<T> GetEvent<T>()
            {
                return _eventsContainer.TryGetValue(typeof(T), out var ret) ? (Event<T>)ret : null;
            }

            public Event<T> GetOrAddEvent<T>()
            {
                var type = typeof(T);
                if (!_eventsContainer.ContainsKey(type))
                {
                    Event<T> newAction = new();
                    _eventsContainer.Add(type, newAction);
                }

                return (Event<T>)_eventsContainer[type];
            }

            public void Remove(Type type)
            {
                _eventsContainer.Remove(type);
            }
        }
    }
}
