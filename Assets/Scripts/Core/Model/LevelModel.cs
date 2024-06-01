using CbUtils;
using Shuile.Core.Framework;
using Shuile.Gameplay;
using System.Collections.Generic;

namespace Shuile.Model
{
    /// <summary> data in single level </summary>
    public class LevelModel : IModel
    {
        private readonly ModuleContainer serviceLocator;
        public float musicBpm;
        public float musicOffset;

        public List<IJudgeable> JudgeObjects { get; set; } = new();
        public float currentMusicTime;

        private float dangerScore = 0f;

        public LevelModel(ModuleContainer serviceLocator)
        {
            this.serviceLocator = serviceLocator;
            var currentChart = LevelDataBinder.Instance.ChartData;
            musicBpm = currentChart.time[0].bpm;
            musicOffset = currentChart.time[0].offset;

            DangerScore = 0f;
        }

        public float BpmIntervalInSeconds => 60f / musicBpm;
        public float OffsetInSeconds => musicOffset * 0.001f;
        public int DangerLevel => DangerLevelUtils.GetDangerLevelUnClamped(DangerScore);
        /// <summary> float - old value </summary>
        public EasyEvent<float> OnDangerScoreChange { get; } = new();
        public float DangerScore
        {
            get => dangerScore;
            set
            {
                var oldVal = dangerScore;
                dangerScore = value < 0 ? 0 : value;
                if (oldVal != dangerScore) OnDangerScoreChange.Invoke(oldVal);
            }
        }

        public ModuleContainer GetModule() => serviceLocator;
    }
}
