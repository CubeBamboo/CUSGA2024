using Shuile.Core.Framework;
using Shuile.Gameplay;
using Shuile.Model;

namespace Shuile.Rhythm
{
    public class LevelTimingManager : ISystem
    {
        private readonly LevelModel levelModel;
        public LevelTimingManager()
        {
            levelModel = this.GetModel<LevelModel>();
        }

        public LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;

        public float GetRealTime(float rhythmTime)
        {
            return levelModel.BpmIntervalInSeconds * rhythmTime;
        }
    }
}
