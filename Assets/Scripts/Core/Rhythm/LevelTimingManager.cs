using Shuile.Gameplay;

namespace Shuile.Rhythm
{
    public class LevelTimingManager
    {
        private readonly LevelModel levelModel;
        public LevelTimingManager()
        {
            levelModel = GameplayService.Interface.Get<LevelModel>();
        }

        public float GetRealTime(float rhythmTime)
        {
            return levelModel.BpmIntervalInSeconds * rhythmTime;
        }
    }
}
