using Shuile.Gameplay;
using Shuile.Rhythm.Runtime;

namespace Shuile.Chart
{
    public class BaseNoteData : IRhythmable
    {
        /// <summary> unit: in chart time </summary>
        public float rhythmTime;

        public static BaseNoteData Create(float time)
            => new() { rhythmTime = time };

        public override string ToString()
        {
            return $"{GetType().Name} targetTime: {rhythmTime}";
        }
    }
}
