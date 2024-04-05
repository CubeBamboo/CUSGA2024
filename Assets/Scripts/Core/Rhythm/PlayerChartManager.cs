using CbUtils;
using UnityEngine;

namespace Shuile.Rhythm
{

    // manage chart of player, convert chart to runtime note object noteContainer
    public class PlayerChartManager : MonoSingletons<PlayerChartManager>
    {
        private NoteContainer noteContainer = new();

        // chart part
        private readonly ChartData chart = ChartDataCreator.CreatePlayerDefault();
        private float[] absoluteTimeChartLoopPart;
        private int loopCount = 0;
        int nextNoteIndex = 0;

        private void Start()
        {
            // convert current chart to absolute time chart
            UpdateAbsoluteTimeChart();
        }

        private void FixedUpdate()
        {
            // if next note time is less than current time, add note to noteContainer and trigger some event
            float singleLoopInterval = chart.length * MusicRhythmManager.Instance.BpmInterval;
            float nextNoteTime = absoluteTimeChartLoopPart[nextNoteIndex] + loopCount * singleLoopInterval;
            if (MusicRhythmManager.Instance.CurrentTime > nextNoteTime)
            {
                noteContainer.AddNote(nextNoteTime);
                nextNoteIndex++;
                if (nextNoteIndex >= absoluteTimeChartLoopPart.Length) //enter next loop
                {
                    loopCount++;
                    nextNoteIndex %= absoluteTimeChartLoopPart.Length;
                }
            }

            noteContainer.UpdateNotePool(MusicRhythmManager.Instance.CurrentTime);
        }

        private void UpdateAbsoluteTimeChart()
        {
            absoluteTimeChartLoopPart = ChartDataUtils.BeatTime2AbsoluteTimeChart(chart, MusicRhythmManager.Instance.BpmInterval);
        }

        public int Count => noteContainer.Count;

        public SingleNote TryGetNearestNote() => noteContainer.TryGetNearestNote();
        public void HitNote(SingleNote note) => noteContainer.ReleseNote(note);
    }
}
