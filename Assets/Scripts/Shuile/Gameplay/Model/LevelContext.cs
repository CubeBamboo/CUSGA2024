using Shuile.Chart;
using Shuile.Core.Gameplay.Data;
using Shuile.Rhythm;

namespace Shuile.Gameplay.Model
{
    public class LevelContext
    {
        public LevelTimingManager TimingManager;
        public LevelData LevelData { get; set; }
        
        private ChartData _chartData;
        
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
