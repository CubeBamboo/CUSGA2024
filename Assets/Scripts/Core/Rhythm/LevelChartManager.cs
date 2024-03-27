using CbUtils;
using Shuile.Rhythm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    //TODO : abstract class

    // play chart for single level
    // control enemy spawn and other event
    // it will auto play.
    public class LevelChartManager : MonoSingletons<LevelChartManager>
    {
        public bool isPlay = true;

        private NoteContainer noteContainer = new();

        // chart part
        private readonly ChartData chart = ChartDataCreator.CreateLevelBreak();
        private float[] absoluteTimeChartLoopPart;
        private int loopCount = 0;
        int nextNoteIndex = 0;

        private void Start()
        {
            UpdateAbsoluteTimeChart();
        }

        private void FixedUpdate()
        {
            if (!isPlay) return;

            // if next note time is less than current time, add note to noteContainer and trigger some event
            float singleLoopInterval = chart.chartLoopLength * MusicRhythmManager.Instance.BpmInterval;
            float nextNoteTime = absoluteTimeChartLoopPart[nextNoteIndex] + loopCount * singleLoopInterval;
            if (MusicRhythmManager.Instance.CurrentTime > nextNoteTime)
            {
                // process note event
                NoteEventHelper.Process(chart.chartLoopPart[nextNoteIndex]); // TODO: Absolute2BeatTimeTime......?

                // note play logic
                nextNoteIndex++;
                if (nextNoteIndex >= absoluteTimeChartLoopPart.Length) //enter next loop
                {
                    loopCount++;
                    nextNoteIndex %= absoluteTimeChartLoopPart.Length;
                }
            }
        }

        private void UpdateAbsoluteTimeChart()
        {
            absoluteTimeChartLoopPart = ChartDataUtils.BeatTime2AbsoluteTimeChart(chart, MusicRhythmManager.Instance.BpmInterval);
        }

        public int Count => noteContainer.Count;
    }
}
