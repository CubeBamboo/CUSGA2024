namespace Shuile.Chart
{
    public class BaseNoteData
    {
        /// <summary> unit: in chart time </summary>
        public float rhythmTime;

        public static BaseNoteData Create(float time)
        {
            return new BaseNoteData { rhythmTime = time };
        }

        public override string ToString()
        {
            return $"{GetType().Name} targetTime: {rhythmTime}";
        }
    }
}
