using JetBrains.Annotations;
using System.Collections.Generic;

namespace Shuile.Rhythm
{
    /* supported chart type: 
     * 1. fixed bpm timing
     */
    public class ChartData
    {
        public class Time
        {
            public int[] beat;
            public float bpm;
        }

        /// <summary> (unit: music bar) </summary>
        public NoteData[] note;
    }

    public static class ChartDataCreator
    {
        public static ChartData CreatePlayerDefault()
        {
            const int size = 10000;
            var chart = new ChartData()
            {
                note = new NoteData[size]
            };
            for (int i = 0; i < size; i++)
            {
                chart.note[i] = NoteData.Create(i + 0f / 4f);
            }
            return chart;
        }

        public static ChartData CreateLevelDefault()
        {
            var newChart = new ChartData
            {
                note = new NoteData[2]
            };

            // init chart

            newChart.note[0] = new NoteData
            {
                targetTime = 0 + 0f / 4f,
                eventType = NoteEventType.SingleEnemySpawn,
            };

            newChart.note[1] = new NoteData
            {
                targetTime = 4 + 0f / 4f,
                eventType = NoteEventType.SingleEnemySpawn,
            };

            return newChart;
        }

        //TODO: try to chart song "Kero Kero Bonito - Break"
        public static ChartData CreateLevelBreak()
        {
            var newChart = new ChartData
            {
                //chartLoopPart = new NoteData[1000] // record every single note // -> use list.ToArray()
            };

            // init chart and add note data
            List<NoteData> noteList = new List<NoteData>
            {
                new NoteData
                {
                    targetTime = 0 + 0f / 4f,
                    eventType = NoteEventType.MusicOffsetTestLaser,
                },
                //new NoteData
                //{
                //    targetTime = 4 + 0f / 4f,
                //    eventType = NoteEventType.SingleEnemySpawn,
                //},
                new NoteData
                {
                    targetTime = 0 + 0f / 4f,
                    endTime = 1024 + 0f / 4f,
                    eventType = NoteEventType.MultiEnemySpawn,
                },
                /*new NoteData
                {
                    targetTime = 7 + 0f / 4f,
                    eventType = NoteEventType.LaserSpawn,
                },
                new NoteData
                {
                    targetTime = 8 + 0f / 4f,
                    eventType = NoteEventType.LaserSpawn,
                },
                new NoteData
                {
                    targetTime = 11 + 0f / 4f,
                    eventType = NoteEventType.LaserSpawn,
                },*/
                //new NoteData
                //{
                //    targetTime = 16 + 0f / 4f,
                //    endTime = 47 + 0f / 4f,
                //    eventType = NoteEventType.MultiEnemySpawn,
                //},
                /*new NoteData
                {
                    targetTime = 32 + 0f / 4f,
                    eventType = NoteEventType.LaserSpawn,
                },
                new NoteData
                {
                    targetTime = 33 + 0f / 4f,
                    eventType = NoteEventType.LaserSpawn,
                },
                new NoteData
                {
                    targetTime = 34 + 0f / 4f,
                    eventType = NoteEventType.LaserSpawn,
                },
                new NoteData
                {
                    targetTime = 40 + 0f / 4f,
                    eventType = NoteEventType.LaserSpawn,
                },
                new NoteData
                {
                    targetTime = 44 + 0f / 4f,
                    eventType = NoteEventType.LaserSpawn,
                },*/
            };

            noteList.Sort((a, b)=> a.targetTime.CompareTo(b.targetTime));
            newChart.note = noteList.ToArray();
            return newChart;
        }
    }

    public static class ChartDataUtils
    {
        /// <summary>convert chart time from beat time to absolute time</summary>
        public static float[] BeatTime2AbsoluteTimeChart(ChartData chart, float bpmInterval)
        {
            float[] absoluteTimeChartLoopPart = new float[chart.note.Length];
            for (int i = 0; i < chart.note.Length; i++)
            {
                absoluteTimeChartLoopPart[i] = chart.note[i].targetTime * bpmInterval;
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
