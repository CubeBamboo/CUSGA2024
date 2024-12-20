using System.Linq;
using System.Text;
using UnityEngine;

namespace Shuile.Chart
{
    /* supported chart type:
     * 1. fixed bpm timing
     */
    public class ChartData
    {
        public AudioClip audioClip;

        public float musicLength;

        /// <summary> (unit: music bar) </summary>
        public BaseNoteData[] note;

        public TimingPoint[] time;

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append("ChartData: ");
            sb.AppendLine(audioClip.name);
            sb.AppendLine($"length: {musicLength}");
            sb.AppendLine($"bpm: {time[0].bpm}, offset: {time[0].offset}");
            sb.AppendLine("NoteData:");
            foreach (var singleNote in note)
            {
                sb.Append("- ");
                sb.Append(singleNote);
                sb.Append("\n");
            }

            sb.Append("\n");
            return sb.ToString();
        }

        #region InternalClass

        public class TimingPoint
        {
            public float bpm;

            /// <summary> (unit: millionseconds) </summary>
            public float offset;
        }

        #endregion
    }

    public static class ChartDataCreator
    {
        public static ChartData CreatePlayerDefault()
        {
            const int size = 10000;
            var chart = new ChartData { note = new BaseNoteData[size] };
            for (var i = 0; i < size; i++)
            {
                chart.note[i] = BaseNoteData.Create(i + (0f / 4f));
            }

            return chart;
        }

        public static ChartData CreateLatencyTestDefault()
        {
            const int size = 8;
            return new ChartData
            {
                note = Enumerable.Range(1, size).Select(i => BaseNoteData.Create((i * 2) + (0f / 4f))).ToArray()
            };
        }

        /*public static ChartData CreateLevelDefault()
        {
            var newChart = new ChartData
            {
                note = new BaseNoteData[2]
            };

            // init chart

            newChart.note[0] = new SpawnSingleEnemyNoteData
            {
                targetTime = 0 + 0f / 4f,
                //eventType = NoteEventType.SingleEnemySpawn,
            };

            newChart.note[1] = new SpawnSingleEnemyNoteData
            {
                targetTime = 4 + 0f / 4f,
                //eventType = NoteEventType.SingleEnemySpawn,
            };

            return newChart;
        }*/
    }

    public static class ChartDataUtils
    {
        /// <summary>convert chart time from beat time to absolute time</summary>
        public static float[] BeatTime2AbsoluteTimeChart(ChartData chart, float bpmInterval)
        {
            var absoluteTimeChartLoopPart = new float[chart.note.Length];
            for (var i = 0; i < chart.note.Length; i++)
            {
                absoluteTimeChartLoopPart[i] = chart.note[i].rhythmTime * bpmInterval;
                //absoluteTimeChartLoopPart[i] = BeatTime2AbsoluteTime(chart.chartLoopPart[i].targetTime, bpmInterval);
            }

            return absoluteTimeChartLoopPart;
        }

        /// <param name="countInMusicBar"></param>
        /// <param name="timeInBeatSnap"></param>
        /// <param name="bpmIntervalInMS"></param>
        public static float BeatTime2AbsoluteTime(uint countInMusicBar, float timeInBeatSnap, float bpmIntervalInMS)
        {
            return (countInMusicBar + timeInBeatSnap) * bpmIntervalInMS;
        }
    }
}
