namespace Shuile.Core.Framework
{
    public static class ServiceLocatorExtension
    {
        public static ServiceLocator AddImplemenation<T>(this ServiceLocator serviceLocator, System.Func<ServiceLocator, T> implementation)
        {
            serviceLocator.RegisterCreator(implementation);
            return serviceLocator;
        }
    }
}
