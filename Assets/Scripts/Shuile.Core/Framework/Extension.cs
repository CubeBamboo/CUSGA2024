namespace Shuile.Core.Framework
{
    public static class Extension
    {
        public static ServiceLocator AddImplemenation<T>(this ServiceLocator serviceLocator, System.Func<T> implementation)
        {
            serviceLocator.RegisterCreator(implementation);
            return serviceLocator;
        }

        public static LayerableServiceLocator AddModelImplemenation<T>(this LayerableServiceLocator serviceLocator, System.Func<T> implementation) where T : IModel
        {
            serviceLocator.AddModelCreator(implementation);
            return serviceLocator;
        }
        public static LayerableServiceLocator AddSystemImplemenation<T>(this LayerableServiceLocator serviceLocator, System.Func<T> implementation) where T : ISystem
        {
            serviceLocator.AddSystemCreator(implementation);
            return serviceLocator;
        }
    }
}
