using Shuile.Core.Configuration;
using Shuile.Core.Framework;
using Shuile.Model;
using UnityEngine;

namespace Shuile.Rhythm.Runtime
{
    public static class ChartManagerCommands
    {
        public static bool TryHitNote(float inputTime, out float hitOffset, LayerableServiceLocator serviceLocator)
        {
            MusicRhythmManager musicRhythmManager = serviceLocator.GetSystem<MusicRhythmManager>();
            PlayerChartManager playerChartManager = serviceLocator.GetSystem<PlayerChartManager>();

            float missTolerance = ImmutableConfiguration.Instance.MissToleranceInSeconds;

            // get the nearest note's time and judge
            hitOffset = float.NaN;
            SingleNote targetNote = playerChartManager.TryGetNearestNote(musicRhythmManager.CurrentTime);
            if (targetNote == null)
                return false;

            bool ret = Mathf.Abs(inputTime - targetNote.realTime) < missTolerance;
            if (ret)
            {
                playerChartManager.HitNote(targetNote);
                hitOffset = inputTime - targetNote.realTime;
            }
            return ret;
        }
    }
}
