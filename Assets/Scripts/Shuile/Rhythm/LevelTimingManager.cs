using Shuile.Framework;
using Shuile.Model;

namespace Shuile.Rhythm
{
    public class LevelTimingManager
    {
        private readonly LevelModel.TimingData _levelModel;

        public LevelTimingManager(RuntimeContext context)
        {
            _levelModel = context.GetImplementation<LevelModel.TimingData>();
        }

        public float GetRealTime(float rhythmTime)
        {
            return _levelModel.BpmIntervalInSeconds * rhythmTime;
        }
    }
}
