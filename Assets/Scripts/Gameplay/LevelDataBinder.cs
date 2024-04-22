using CbUtils;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    // store data of current level
    // TODO: use model
    public class LevelDataBinder : MonoSingletons<LevelDataBinder>
    {
        private LevelData levelData;

        // scene will pre set-up in unity's scene file
        public LevelEnemySO levelEnemyData => levelData.enemyData;
        public ChartSO chartFiles => levelData.chartFiles;

        public ChartData chartData;
        public ChartData ChartData => chartData ??= ChartUtils.LoadChartSync(chartFiles);

        public void Initialize()
        {
            chartData = ChartUtils.LoadChartSync(chartFiles);
        }
        public void DeInitialize()
        {
            chartData = null;
        }

        public void SetLevelData(LevelData levelData)
        {
            this.levelData = levelData;
        }
    }
}
