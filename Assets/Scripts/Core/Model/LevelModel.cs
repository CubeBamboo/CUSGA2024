using CbUtils;
using Shuile.Framework;
using Shuile.Gameplay;

using System.Collections.Generic;

namespace Shuile
{
    /// <summary> data in single level </summary>
    public class LevelModel : IModel
    {
        public float musicBpm;
        public float musicOffset;
        public List<IJudgeable> JudgeObjects { get; set; } = new();
        //public EasyEvent OnPlayerHit { get; set; } = new(); // rhythm hit

        private float missTolerance;

        private float dangerScore = 0f;

        public LevelModel()
        {
            var resources = LevelResources.Instance;
            missTolerance = resources.levelConfig.missTolerance;

            var currentChart = LevelDataBinder.Instance.ChartData;
            musicBpm = currentChart.time[0].bpm;
            musicOffset = currentChart.time[0].offset;

            DangerScore = 0f;
        }

        public float BpmIntervalInSeconds => 60f / musicBpm;
        public float OffsetInSeconds => musicOffset * 0.001f;
        public float MissToleranceInSeconds => missTolerance * 0.001f;
        public int DangerLevel => DangerLevelUtils.GetDangerLevelUnClamped(DangerScore);
        public float DangerScore
        {
            get => dangerScore;
            set => dangerScore = value < 0 ? 0 : value;
        }
    }
}
