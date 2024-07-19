using Shuile.Chart;
using Shuile.Core.Gameplay.Data;

namespace Shuile.Gameplay.Model
{
    public class LevelContext
    {
        private ChartData _chartData;
        public LevelData LevelData { get; set; }

        public ChartSO ChartFiles => LevelData.chartFiles;
        public LevelEnemySO LevelEnemyData => LevelData.enemyData;
        public ChartData ChartData => _chartData ??= ChartUtils.LoadChartSync(ChartFiles);

        public void Initialize()
        {
            _chartData = ChartUtils.LoadChartSync(ChartFiles);
        }

        public void DeInitialize()
        {
            _chartData = null;
        }
    }
}
