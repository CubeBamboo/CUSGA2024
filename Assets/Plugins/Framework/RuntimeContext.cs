using JetBrains.Annotations;
using Shuile.Core.Framework;

namespace Plugins.Framework
{
    public class RuntimeContext
    {
        private ServiceLocator serviceLocator;
        public ServiceLocator ServiceLocator => serviceLocator ??= new ServiceLocator();

        [CanBeNull] public MonoContainer Reference { get; set; }
    }
}
