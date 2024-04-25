using System;

namespace CbUtils
{
    public abstract class CSharpLazySingletons<T> where T : new()
    {
        private static readonly Lazy<T> _instance
            = new Lazy<T>(() => new T());
        public static T Instance => _instance.Value;
    }

    public abstract class CSharpHungrySingletons<T> where T : new()
    {
        public static T Instance { get; } = new T();
    }
}
