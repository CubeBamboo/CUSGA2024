using CbUtils.Extension;
using Shuile.Chart;
using Shuile.Core.Gameplay.Data;
using Shuile.Gameplay;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Manager;
using UnityEngine;

namespace Shuile.Rhythm.Runtime
{
    public class SpawnLaserNoteData : BaseNoteData
    {
        public override void Process()
        {
            LevelEntityFactory.Instance.SpawnLaser()
                .SetPosition(LevelZoneManager.Instance.RandomValidPosition());
        }

        public override float ToPlayTime()
            => this.GetRealTime(rhythmTime - Laser.InTime, LevelRoot.LevelContext.timingManager);
    }

    public class SpawnSingleEnemyNoteData : BaseNoteData
    {
        public override void Process()
        {
            var randomType = (EnemyType)Random.Range(0, (int)EnemyType.TotalCount);
            LevelEntityFactory.Instance.SpawnEnemy(randomType, LevelZoneManager.Instance.RandomValidPosition());
        }
    }
}
