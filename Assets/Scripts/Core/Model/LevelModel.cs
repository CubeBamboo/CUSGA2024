using Shuile.Framework;

namespace Shuile
{
    /// <summary> data in single level </summary>
    public class LevelModel : IModel
    {
        public float musicBpm;
        public float musicOffset;
        public System.Action onRhythmHit;

        public float BpmIntervalInSeconds => 60f / musicBpm;
        public float OffsetInSeconds => musicOffset * 0.001f;
    }
}
