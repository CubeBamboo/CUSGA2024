using CbUtils;
using Shuile.Core.Gameplay;
using Shuile.Core.Rhythm;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;

namespace Shuile
{
    public class LevelContext
    {
        public LevelTimingManager timingManager;
        public LevelData LevelData { get; set; }
        
        private ChartData chartData;
        
        public ChartSO chartFiles => LevelData.chartFiles;
        public LevelEnemySO levelEnemyData => LevelData.enemyData;
        public ChartData ChartData => chartData ??= ChartUtils.LoadChartSync(chartFiles);
        
        public void Initialize()
        {
            chartData = ChartUtils.LoadChartSync(chartFiles);
        }
        public void DeInitialize()
        {
            chartData = null;
        }
    }
}
