using Shuile.Gameplay;
using UnityEngine;

namespace Shuile.Rhythm.Runtime
{
    public static class MusicRhythmManagerExt
    {
        /// <summary>
        /// beatTime = barTime + timeInSingleBar, {barTime}th beat and {timeInSingleBar} position in this bar,
        /// {timeInSingleBar} should be in [0, 1)
        /// </summary>
        /// <returns> time in seconds </returns>
        public static float GetRealTime(this MusicRhythmManager rhythmManager, float rhythmTime)
        {
            return rhythmManager.levelModel.BpmIntervalInSeconds * rhythmTime;
        }
    }
}
