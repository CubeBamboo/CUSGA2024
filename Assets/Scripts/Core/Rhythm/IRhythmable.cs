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
        public static float GetRealTime(this IRhythmable rhythmable, float rhythmTime)
            => MusicRhythmManager.Instance.GetRealTime(rhythmTime);

        /// <summary> {barTime}th beat and {timeInSingleBar} position in this bar </summary>
        /// <returns> time in seconds </returns>
        public static float GetRealTime(this IRhythmable rhythmable, float barTime, float timeInSingleBar)
            => GetRealTime(rhythmable, barTime + timeInSingleBar);
    }
}
