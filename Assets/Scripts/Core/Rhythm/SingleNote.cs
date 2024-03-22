// encoding: UTF-8 cp65001

namespace Shuile.Rhythm
{
    // note object spawn in game runtime
    public class SingleNote
    {
        public float targetTime;

        public SingleNote(float targetTime)
        {
            this.targetTime = targetTime;
        }

        public bool NeedRelese(float currentTime, float missTolerance)
        {
            return currentTime - targetTime > missTolerance;
        }
    }
}
