using Shuile.Chart;
using Shuile.Core.Gameplay.Data;

namespace Shuile.Gameplay.Model
{
    // runtime level data
    public class SingleLevelData
    {
        public LevelData LevelData { get; set; }

        public SingleLevelData(LevelData data)
        {
            LevelData = data;
            ChartData = ChartUtils.LoadChartSync(data.chartFiles);
        }

        public ChartData ChartData { get; set; }
        public string SongName => LevelData.songName;
        public string Composer => LevelData.composer;
        public LevelEnemySO LevelEnemyData => LevelData.enemyData;
    }
}
