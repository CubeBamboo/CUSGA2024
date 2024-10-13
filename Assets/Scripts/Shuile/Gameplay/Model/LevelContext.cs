using Shuile.Chart;
using Shuile.Core.Gameplay.Data;

namespace Shuile.Gameplay.Model
{
    // -- will be refactored in the future
    public class LevelContext
    {
        private ChartData _chartData;
        public LevelData LevelData { get; set; }

        public ChartSO ChartFiles => LevelData.chartFiles;
        public LevelEnemySO LevelEnemyData => LevelData.enemyData;
        public ChartData ChartData => _chartData ??= ChartUtils.LoadChartSync(ChartFiles);

        public LevelContext(LevelData data)
        {
            LevelData = data;
        }
    }
}
