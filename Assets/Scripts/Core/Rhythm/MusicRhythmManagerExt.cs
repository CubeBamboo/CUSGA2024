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
        public static float GetRhythmTime(this MusicRhythmManager rhythmManager, float beatTime)
        {
            return rhythmManager.levelModel.BpmIntervalInSeconds * beatTime;
        }

        /// <summary>
        /// check if input in current time is in beat (quarter step).
        /// </summary>
        /// <param name="hitOffset">return float.NaN if not hit on</param>
        public static bool CheckBeatRhythm(this MusicRhythmManager rhythmManager, float inputTime, out float hitOffset)
        {
            float missTolerance = GameplayService.Interface.LevelModel.MissToleranceInSeconds;

            // get the nearest note's time and judge
            hitOffset = float.NaN;
            SingleNote targetNote = PlayerChartManager.Instance.TryGetNearestNote();
            if(targetNote == null)
                return false;

            bool ret = Mathf.Abs(inputTime - targetNote.targetTime) < missTolerance;
            if(ret)
            {
                PlayerChartManager.Instance.HitNote(targetNote);
                hitOffset = inputTime - targetNote.targetTime;
            }
            return ret;
        }
    }
}
