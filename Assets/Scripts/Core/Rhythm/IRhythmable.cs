namespace Shuile.Rhythm.Runtime
{
    public interface IRhythmable
    {
    }

    public static class IRhythmableExtension
    {
        /// <summary>
        /// beatTime = barTime + timeInSingleBar, {barTime}th beat and {timeInSingleBar} position in this bar,
        /// {timeInSingleBar} should be in [0, 1)
        /// </summary>
        /// <returns> time in seconds </returns>
        public static float GetRhythmTime(this IRhythmable rhythmable, float beatTime)
            => MusicRhythmManager.Instance.GetRhythmTime(beatTime);

        /// <summary> {barTime}th beat and {timeInSingleBar} position in this bar </summary>
        /// <returns> time in seconds </returns>
        public static float GetRhythmTime(this IRhythmable rhythmable, float barTime, float timeInSingleBar)
            => GetRhythmTime(rhythmable, barTime + timeInSingleBar);
    }
}
