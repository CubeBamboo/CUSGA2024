using Shuile.Chart;
using Shuile.Core.Gameplay.Data;
using Shuile.UI.Data;

namespace Shuile.Gameplay.Model
{
    // -- will be refactored in the future
    public class SingleLevelData
    {
        private ChartData _chartData;
        public LevelData LevelData { get; set; }

        public ChartSO ChartFiles => LevelData.chartFiles;
        public LevelEnemySO LevelEnemyData => LevelData.enemyData;
        public ChartData ChartData => _chartData ??= ChartUtils.LoadChartSync(ChartFiles);

        public string SongName { get; set; }
        public string Composer { get; set; }

        public SingleLevelData(LevelData data)
        {
            LevelData = data;
        }

        public SingleLevelData(LevelSelectDataSO.Data data)
        {
            var levelData = GameApplication.BuiltInData.levelDataMap.FirstByLabel(data.levelDataLabel);
            LevelData = levelData;
            SongName = data.songName;
            Composer = data.songArtist;
        }
    }
}
