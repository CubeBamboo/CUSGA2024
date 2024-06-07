using Shuile.Chart;
using Shuile.Core.Framework;
using Shuile.Core.Global.Config;
using UnityEngine;

namespace Shuile.Rhythm.Runtime
{
    internal class TryHitNoteCommand : ICommand
    {
        internal struct Result
        {
            public float hitOffset;
            public bool isHitOn;
        }

        public float inputTime;
        public MusicRhythmManager musicRhythmManager;
        public PlayerChartManager playerChartManager;

        public Result result { get; private set; }

        public void Execute()
        {
            Result result = new();
            float missTolerance = ImmutableConfiguration.Instance.MissToleranceInSeconds;

            // get the nearest note's time and judge
            result.hitOffset = float.NaN;
            SingleNote targetNote = playerChartManager.TryGetNearestNote(musicRhythmManager.CurrentTime);
            if (targetNote == null)
            {
                result.isHitOn = false;
                return;
            }

            bool ret = Mathf.Abs(inputTime - targetNote.realTime) < missTolerance;
            if (ret)
            {
                playerChartManager.HitNote(targetNote);
                result.hitOffset = inputTime - targetNote.realTime;
            }
            result.isHitOn = ret;
            this.result = result;
            return;
        }
    }

    //public static class ChartManagerCommands
    //{
    //    //public static bool TryHitNote(float inputTime, out float hitOffset, ModuleContainer serviceLocator)
    //    //{
    //    //    MusicRhythmManager musicRhythmManager = serviceLocator.GetSystemImplemenation<MusicRhythmManager>();
    //    //    PlayerChartManager playerChartManager = serviceLocator.GetSystemImplemenation<PlayerChartManager>();

    //    //    float missTolerance = ImmutableConfiguration.Instance.MissToleranceInSeconds;

    //    //    // get the nearest note's time and judge
    //    //    hitOffset = float.NaN;
    //    //    SingleNote targetNote = playerChartManager.TryGetNearestNote(musicRhythmManager.CurrentTime);
    //    //    if (targetNote == null)
    //    //        return false;

    //    //    bool ret = Mathf.Abs(inputTime - targetNote.realTime) < missTolerance;
    //    //    if (ret)
    //    //    {
    //    //        playerChartManager.HitNote(targetNote);
    //    //        hitOffset = inputTime - targetNote.realTime;
    //    //    }
    //    //    return ret;
    //    //}
    //}
}
