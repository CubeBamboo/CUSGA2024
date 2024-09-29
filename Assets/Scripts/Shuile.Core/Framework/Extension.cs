using Shuile.Framework;
using System;

namespace Shuile.Core.Framework
{
    public static class Extension
    {
        public static ServiceLocator AddImplemenation<T>(this ServiceLocator serviceLocator, Func<T> implementation)
        {
            serviceLocator.RegisterCreator(implementation);
            return serviceLocator;
        }

        public static LayerableServiceLocator AddModelImplemenation<T>(this LayerableServiceLocator serviceLocator,
            Func<T> implementation) where T : IModel
        {
            serviceLocator.AddModelCreator(implementation);
            return serviceLocator;
        }

        public static LayerableServiceLocator AddSystemImplemenation<T>(this LayerableServiceLocator serviceLocator,
            Func<T> implementation) where T : ISystem
        {
            serviceLocator.AddSystemCreator(implementation);
            return serviceLocator;
        }
    }
}
