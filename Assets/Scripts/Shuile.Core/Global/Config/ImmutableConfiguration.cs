using CbUtils;
using Shuile.ResourcesManagement.Loader;

namespace Shuile.Core.Configuration
{
    public class ImmutableConfiguration : CSharpLazySingletons<ImmutableConfiguration>
    {
        private readonly float missTolerance;

        public ImmutableConfiguration()
        {
            missTolerance = LevelResourcesLoader.Instance.SyncContext.levelConfig.missTolerance;
        }

        public float MissToleranceInSeconds => missTolerance * 0.001f;
    }
}
