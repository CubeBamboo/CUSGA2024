using Shuile.Gameplay;
using Shuile.Rhythm.Runtime;

namespace Shuile.Chart
{
    public class BaseNoteData : IRhythmable
    {
        /// <summary> unit: in chart time </summary>
        public float rhythmTime;
        public virtual void Process() { }
        public virtual float ToPlayTime()
            => this.GetRealTime(rhythmTime, LevelRoot.LevelContext.timingManager);

        public static BaseNoteData Create(float time)
            => new() { rhythmTime = time };

        public override string ToString()
        {
            return $"{GetType().Name} targetTime: {rhythmTime}";
        }
    }
}
