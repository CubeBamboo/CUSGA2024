using Shuile.Core.Framework.Unity;
using Shuile.Framework;
using Shuile.Model;

namespace Shuile.Rhythm
{
    public class LevelTimingManager
    {
        private readonly LevelModel _levelModel;

        public LevelTimingManager(RuntimeContext context)
        {
            _levelModel = context.GetImplementation<LevelModel>();
        }

        public float GetRealTime(float rhythmTime)
        {
            return _levelModel.BpmIntervalInSeconds * rhythmTime;
        }
    }
}
