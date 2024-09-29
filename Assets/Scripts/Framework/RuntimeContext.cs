using JetBrains.Annotations;

namespace Shuile.Framework
{
    public class RuntimeContext
    {
        private ServiceLocator serviceLocator;
        public ServiceLocator ServiceLocator => serviceLocator ??= new ServiceLocator();

        [CanBeNull] public MonoContainer Reference { get; set; }
    }
}
