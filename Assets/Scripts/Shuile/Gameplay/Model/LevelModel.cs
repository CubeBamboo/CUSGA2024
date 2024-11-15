using CbUtils;
using Shuile.Chart;
using Shuile.Core.Gameplay.Common;
using Shuile.Core.Global.Config;
using System.Collections.Generic;

namespace Shuile.Model
{
    /// <summary> data in single level </summary>
    public class LevelModel
    {
        private float _dangerScore;

        public LevelModel()
        {
            DangerScore = 0f;
        }

        public int DangerLevel => DangerLevelUtils.GetDangerLevelUnClamped(DangerScore);
        public List<IJudgeable> JudgeObjects { get; set; } = new();
        public float CurrentMusicTime { get; set; }

        /// <summary> float - old value </summary>
        public EasyEvent<float> OnDangerScoreChange { get; } = new();

        public float DangerScore
        {
            get => _dangerScore;
            set
            {
                var oldVal = _dangerScore;
                _dangerScore = value < 0 ? 0 : value;
                if (oldVal != _dangerScore)
                {
                    OnDangerScoreChange.Invoke(oldVal);
                }
            }
        }

        public class TimingData
        {
            public ChartData.TimingPoint[] timingPoints;

            public TimingData(ChartData.TimingPoint[] timingPoints)
            {
                this.timingPoints = timingPoints;
                _musicBpm = timingPoints[0].bpm;
                _musicOffset = timingPoints[0].offset;
            }

            private readonly float _musicBpm;
            private readonly float _musicOffset;

            public float BpmIntervalInSeconds => 60f / _musicBpm;
            public float OffsetInSeconds => _musicOffset * 0.001f;
        }
    }
}
