
using CbUtils;
using Shuile.Framework;
using UnityEditorInternal;
using UnityEngine;

namespace Shuile.Rhythm
{
    /* supported chart type: 
     * 1. fixed bpm timing
     * 2. loop in range
     */
    public class ChartData
    {
        /// <summary> (unit: music bar) for loop </summary>
        public float chartLoopLength;
        /// <summary> (unit: music bar) </summary>
        public NoteData[] chartLoopPart;

        public static ChartData CreatePlayerDefault => new ChartData()
        {
            chartLoopLength = 4f,
            chartLoopPart = new NoteData[4]
            {
                NoteData.CreateWithTime(0 + 0f / 4f),
                NoteData.CreateWithTime(1 + 0f / 4f),
                NoteData.CreateWithTime(2 + 0f / 4f),
                NoteData.CreateWithTime(3 + 0f / 4f),
            }
        };

        //TODO: store in file
        public static ChartData CreateLevelDefault
        {
            get
            {
                var newChart = new ChartData
                {
                    chartLoopLength = 4f,
                    chartLoopPart = new NoteData[4]
                };

                for (int i = 0; i < newChart.chartLoopPart.Length; i++)
                {
                    newChart.chartLoopPart[i] = new NoteData()
                    {
                        targetTime = i + 0f / 4f,
                        eventType = NoteEventType.SingleEnemySpawn,
                        eventData = new NoteEventData()
                        {
                            enemyType = EnemyType.Vegetable,
                            enemyPosition = UnityEngine.Vector3.zero,
                        }
                    };
                }

                return newChart;
            }
        }
    }

    public static class ChartDataUtils
    {
        /// <summary>convert chart time from beat time to absolute time</summary>
        public static float[] BeatTime2AbsoluteTimeChart(ChartData chart, float bpmInterval)
        {
            float[] absoluteTimeChartLoopPart = new float[chart.chartLoopPart.Length];
            for (int i = 0; i < chart.chartLoopPart.Length; i++)
            {
                absoluteTimeChartLoopPart[i] = chart.chartLoopPart[i].targetTime * bpmInterval;
            }
            return absoluteTimeChartLoopPart;
        }
    }

    public struct NoteData
    {
        /// <summary>
        /// integer part - beat count, decimal part - where in a beat. 
        /// for example: targetTimeArray = { 0 + 0f / 4f, 1 + 0f / 4f, 2 + 0f / 4f, 3 + 0f / 4f };
        /// </summary>
        public float targetTime;

        public NoteEventType eventType;
        public NoteEventData eventData;

        public static NoteData CreateWithTime(float targetTime)
            => new() { targetTime = targetTime };
    }

    public static class NoteEventHelper
    {
        // type -> process
        public static void Process(NoteData noteData)
        {
            switch (noteData.eventType)
            {
                case NoteEventType.SingleEnemySpawn:
                    SingleEnemySpawn(noteData.eventData);
                    break;
                case NoteEventType.ObjectTransform:
                    throw new System.NotImplementedException();
                default:
                    throw new System.Exception("unknown note event type");
            }
        }

        public static void SingleEnemySpawn(NoteEventData noteData)
        {
            // TODO: store in eventdata
            var offset = new Vector2(0, 0);
            var rectScale = new Vector2(1, 1);
            var pos = offset + new Vector2(rectScale.x * Random.value, rectScale.y * Random.value); // random in a rect

            EnemyType2Prefab(noteData.enemyType).Instantiate().
                SetPosition(pos);
        }

        public static UnityEngine.GameObject EnemyType2Prefab(EnemyType enemyType)
        {
            PrefabConfigSO prefabConfig = MainGame.Interface.Get<PrefabConfigSO>();
            var res = enemyType switch
            {
                EnemyType.Vegetable => prefabConfig.vegetableEnemy,
                EnemyType.Normal => prefabConfig.normalEnemy,
                _ => null,
            };
            return res;
        }
    }

    public enum EnemyType
    {
        Vegetable,
        Normal,
    }

    public enum NoteEventType
    {
        SingleEnemySpawn,
        ObjectTransform,
    }

    public struct NoteEventData
    {
        public EnemyType enemyType;
        public UnityEngine.Vector3 enemyPosition;
    }
}
