using System;

namespace Shuile.Framework
{
    public static class Extensions
    {
        public static IReadOnlyServiceLocator Resolve<T>(this IReadOnlyServiceLocator serviceLocator, out T dest)
        {
            dest = serviceLocator.Get<T>();
            if (dest == null)
            {
                throw new Exception($"Cannot resolve {typeof(T).Name}");
            }

            return serviceLocator;
        }
    }
}
