using System;
using UnityEngine;

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

        public static UnityEntryPointScheduler RegisterMonoScheduler(this ServiceLocator serviceLocator, MonoBehaviour monoBehaviour)
        {
            UnityEntryPointScheduler scheduler;
            serviceLocator.RegisterInstance(scheduler = UnityEntryPointScheduler.Create(monoBehaviour));
            return scheduler;
        }
    }
}
