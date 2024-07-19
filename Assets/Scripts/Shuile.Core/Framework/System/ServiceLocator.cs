using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Shuile.Core.Framework
{
    public class ServiceLocator
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

        public void AddServiceDirectly<T>(T service)
        {
            AddServiceDirectly(typeof(T), service);
        }

        public void AddServiceDirectly(Type type, object service)
        {
            _services[type] = service;
        }

        [DebuggerHidden]
        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        [DebuggerHidden]
        public object GetService(Type type)
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

        public void ClearAllServices()
        {
            _services.Clear();
        }

        public void ClearAllServicesCreator()
        {
            _serviceCreators.Clear();
        }

        public bool ContainsService(object instance)
        {
            return _services.ContainsValue(instance);
        }

        public bool ContainsService<T>()
        {
            return _services.ContainsKey(typeof(T));
        }

        public bool ContainsService(Type type)
        {
            return _services.ContainsKey(type);
        }
    }
}
