using UnityEngine;

namespace Shuile
{
    public static class MusicRhythmManagerExt
    {
        /// <summary>
        /// check if input in current time is in beat (quarter step).
        /// </summary>
        /// <param name="hitOffset">return float.NaN if not hit on</param>
        public static bool CheckBeatRhythm(this MusicRhythmManager rhythmManager, out float hitOffset)
        {
            float missTolerance = rhythmManager.MissTolerance * 0.001f;
            float bpmInterval = rhythmManager.BpmInterval;

            // get the nearest note's time
            float inputTime = rhythmManager.CurrentTime; // no need to add offset
            float progress = inputTime % bpmInterval;

            // TODO : check if note has been hit...
            hitOffset = float.NaN;

            // hit this beat note
            if (progress < missTolerance)
            {
                hitOffset = progress;
                return true;
            }

            // hit next beat note
            if (bpmInterval - progress < missTolerance)
            {
                hitOffset = progress - bpmInterval;
                return true;
            }
            return false;
        }
    }
}
