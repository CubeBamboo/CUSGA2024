using CbUtils;
using Shuile.Rhythm.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    // init level data
    [System.Obsolete("no use")]
    public class LevelManager : MonoSingletons<LevelManager>
    {
        public bool isPlay = true;
        protected override void OnAwake()
        {
            LevelChartManager.Instance.enabled = true;
            LevelChartManager.Instance.isPlay = isPlay;
            EnemyRoundManager.Instance.enabled = true;
        }
    }
}
