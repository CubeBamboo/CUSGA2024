using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Shuile.Core.Framework
{
    public class ServiceLocator
    {
        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<Type, Func<object>> _serviceCreators = new();

        public void RegisterCreator<T>(Func<T> creator)
        {
            _serviceCreators[typeof(T)] = () => creator();
        }

        public void UnRegisterCreator<T>()
        {
            _serviceCreators.Remove(typeof(T));
        }
        
        public void AddServiceDirectly<T>(T service)
        {
            _services[typeof(T)] = service;
        }

        [DebuggerHidden]
        public T GetService<T>()
        {
            Type type = typeof(T);
            if (_services.TryGetValue(type, out var obj))
            {
                return (T)obj;
            }
            else if(_serviceCreators.TryGetValue(type, out var cre))
            {
                var service = (T)cre();
                _services[type] = service;
                return service;
            }

            throw new Exception($"Service creator of type {type} not found");
        }
        public void ClearAllServices() => _services.Clear();
        public void ClearAllServicesCreator() => _serviceCreators.Clear();
        public bool ContainsService(object instance) => _services.ContainsValue(instance);
        public bool ContainsService<T>() => _services.ContainsKey(typeof(T));
        public bool ContainsService(System.Type type) => _services.ContainsKey(type);
    }
}
