using CbUtils;
using Shuile.Core.Global.Config;
using Shuile.Gameplay;
using System.Collections.Generic;

namespace Shuile.Model
{
    /// <summary> data in single level </summary>
    public class LevelModel
    {
        private readonly float _musicBpm;
        private readonly float _musicOffset;
        private float _dangerScore = 0f;

        public LevelModel()
        {
            var currentChart = LevelRoot.LevelContext.ChartData;
            _musicBpm = currentChart.time[0].bpm;
            _musicOffset = currentChart.time[0].offset;

            DangerScore = 0f;
        }

        public float BpmIntervalInSeconds => 60f / _musicBpm;
        public float OffsetInSeconds => _musicOffset * 0.001f;
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
                if (oldVal != _dangerScore) OnDangerScoreChange.Invoke(oldVal);
            }
        }
    }
}
