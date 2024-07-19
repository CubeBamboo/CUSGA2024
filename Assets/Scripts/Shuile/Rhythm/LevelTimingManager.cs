using Shuile.Core.Framework.Unity;
using Shuile.Model;

namespace Shuile.Rhythm
{
    public class LevelTimingManager
    {
        private readonly LevelModel _levelModel;

        public LevelTimingManager(IGetableScope scope)
        {
            _levelModel = scope.GetImplementation<LevelModel>();
        }

        public float GetRealTime(float rhythmTime)
        {
            return _levelModel.BpmIntervalInSeconds * rhythmTime;
        }
    }
}
