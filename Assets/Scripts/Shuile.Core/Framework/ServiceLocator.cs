using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Shuile.Core.Framework
{
    public class ServiceLocator
    {
        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<Type, Func<object>> _serviceCreators = new();

        public void RegisterCreator<T>(Func<ServiceLocator, T> creator)
        {
            _serviceCreators[typeof(T)] = () => creator(this);
        }

        public void UnRegisterCreator<T>()
        {
            _serviceCreators.Remove(typeof(T));
        }

        [DebuggerHidden]
        public T GetService<T>()
        {
            if (_services.TryGetValue(typeof(T), out var obj))
            {
                return (T)obj;
            }
            else if(_serviceCreators.TryGetValue(typeof(T), out var ctr))
            {
                var service = (T)ctr();
                _services[typeof(T)] = service;
                return service;
            }

            throw new Exception($"Service creator of type {typeof(T)} not found");
        }
        public void ClearAllServices() => _services.Clear();
        public void ClearAllServicesCreator() => _serviceCreators.Clear();
    }
}
