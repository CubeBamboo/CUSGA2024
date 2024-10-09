using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace Shuile.Framework
{
    /// <summary>
    /// can be accessed for object which has sibling nodes and child nodes. it contains runtime dependencies.
    /// </summary>
    public class RuntimeContext : IReadOnlyServiceLocator, IServiceLocatorRegister
    {
        private ServiceLocator serviceLocator;

        public RuntimeContext()
        {
            serviceLocator = new ServiceLocator();
        }

        public RuntimeContext(ServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        /// <summary>
        /// for debugging, reference to the container.
        /// </summary>
        [CanBeNull] public object Reference { get; set; }
        [CanBeNull] public List<object> ParentReference { get; set; }

        #region PublicInterface

        public void AddParent(RuntimeContext parent)
        {
            serviceLocator.AddParent(parent.serviceLocator);
            ParentReference ??= new List<object>();
            ParentReference.Add(parent.Reference);
        }

        public T Get<T>()
        {
            return serviceLocator.Get<T>();
        }

        public object Get(Type type)
        {
            return serviceLocator.Get(type);
        }

        public bool Contains(Type type)
        {
            return serviceLocator.Contains(type);
        }

        public bool ContainsInternalWithoutParent(Type type)
        {
            return serviceLocator.ContainsInternalWithoutParent(type);
        }

        public string KeyName
        {
            get => serviceLocator.KeyName;
            set => serviceLocator.KeyName = value;
        }

        public void RegisterFactory<T>(Func<T> func)
        {
            serviceLocator.RegisterFactory(func);
        }

        public void RegisterFactory(Type type, Func<object> func)
        {
            serviceLocator.RegisterFactory(type, func);
        }

        public void UnRegisterFactory<T>()
        {
            serviceLocator.UnRegisterFactory<T>();
        }

        public void UnRegisterFactory(Type type)
        {
            serviceLocator.UnRegisterFactory(type);
        }

        public void RegisterInstance<T>(T service)
        {
            serviceLocator.RegisterInstance(service);
        }

        public void RegisterInstance(Type type, object service)
        {
            serviceLocator.RegisterInstance(type, service);
        }

        #endregion
    }
}
