using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Shuile.Core.Framework
{
    public interface IReadOnlyServiceLocator
    {
        T Get<T>();
        object Get(Type type);
    }

    public class ServiceLocator : IReadOnlyServiceLocator
    {
        private static readonly Type objectType = typeof(object);
        private readonly Dictionary<Type, Func<object>> _serviceCreators = new();

        private readonly Dictionary<Type, object> _services = new();

        public void RegisterCreator<T>(Func<T> creator)
        {
            RegisterCreator(typeof(T), () => creator());
        }

        public void RegisterCreator(Type type, Func<object> creator)
        {
            if (type == objectType)
            {
                throw new ArgumentException("Registering object type is not allowed.");
            }

            _serviceCreators[type] = creator;
        }

        public void UnRegisterCreator<T>()
        {
            UnRegisterCreator(typeof(T));
        }

        public void UnRegisterCreator(Type type)
        {
            _serviceCreators.Remove(type);
        }

        public void AddDirectly<T>(T service)
        {
            AddDirectly(typeof(T), service);
        }

        public void AddDirectly(Type type, object service)
        {
            _services[type] = service;
        }

        [DebuggerHidden]
        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }

        [DebuggerHidden]
        public object Get(Type type)
        {
            return GetInternal(type);
        }

        public ServiceLocator Resolve<T>(out T dest)
        {
            dest = Get<T>();
            return this;
        }

        private object GetInternal(Type type)
        {
            if (_services.TryGetValue(type, out var obj))
            {
                return obj;
            }

            if (_serviceCreators.TryGetValue(type, out var cre))
            {
                var service = cre();
                _services[type] = service;
                return service;
            }

            throw new Exception($"Service creator of type {type} not found");
        }

        public void ClearAll()
        {
            _services.Clear();
        }

        public void ClearAllCreator()
        {
            _serviceCreators.Clear();
        }

        public bool Contains(object instance)
        {
            return ContainsInternal(instance);
        }

        private bool ContainsInternal(object instance)
        {
            return _services.ContainsValue(instance);
        }

        public bool Contains<T>()
        {
            return _services.ContainsKey(typeof(T));
        }

        public bool Contains(Type type)
        {
            return _services.ContainsKey(type);
        }
    }
}
