using CbUtils;
using Shuile.Rhythm;
using UnityEngine;

namespace Shuile
{
    // manage auto play chart. (for someone like enemy or game ui animation)
    public class AutoPlayChartManager : MonoSingletons<AutoPlayChartManager>
    {
        private NoteContainer noteContainer = new();

        // chart part
        private readonly ChartData chart = ChartDataCreator.CreatePlayerDefault();
        private float[] absoluteTimeChartLoopPart;
        private int loopCount = 0;
        int nextNoteIndex = 0;

        /// <summary>
        /// call when a beat is hit
        /// </summary>
        public event System.Action OnRhythmHit;

        private void Start()
        {
            UpdateAbsoluteTimeChart();
        }

        private void FixedUpdate()
        {
            // if next note time is less than current time, add note to noteContainer and trigger some event
            float singleLoopInterval = chart.chartLoopLength * MusicRhythmManager.Instance.BpmInterval;
            float nextNoteTime = absoluteTimeChartLoopPart[nextNoteIndex] + loopCount * singleLoopInterval;
            if (MusicRhythmManager.Instance.CurrentTime > nextNoteTime)
            {
                OnRhythmHit?.Invoke();
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
