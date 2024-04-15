using CbUtils;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    // store data of current level
    public class LevelDataBinder : MonoSingletons<LevelDataBinder>
    {
        [SerializeField] private string levelLabel; // only for debug
        public EnemyRoundsSO enemyRoundsData;
        public ChartSO chartFiles;
        // scene will pre set-up in unity's scene file

        public ChartData chartData { get; private set; }

        protected override void OnAwake()
        {
            chartData = ChartUtils.LoadChartSync(chartFiles);
        }
    }
}
