using System;

namespace Shuile.Core.Framework
{
    public class LayerableServiceLocator
    {
        private readonly ServiceLocator _serviceLocator = new();

        public void AddUtilityCreator<T>(Func<T> creator) where T : IUtility
        {
            _serviceLocator.RegisterCreator(creator);
        }

        public void AddModelCreator<T>(Func<T> creator) where T : IModel
        {
            _serviceLocator.RegisterCreator(creator);
        }

        public void AddSystemCreator<T>(Func<T> creator) where T : ISystem
        {
            _serviceLocator.RegisterCreator(creator);
        }

        public T GetUtility<T>() where T : IUtility
        {
            return _serviceLocator.GetService<T>();
        }

        public T GetModel<T>() where T : IModel
        {
            return _serviceLocator.GetService<T>();
        }

        public T GetSystem<T>() where T : ISystem
        {
            return _serviceLocator.GetService<T>();
        }

        public void ClearExisting()
        {
            _serviceLocator.ClearAllServices();
        }
    }
}
