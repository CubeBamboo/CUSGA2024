// [WIP]

using UnityEngine;

using CbUtils.Extension;
using Shuile.Core.Gameplay;
using Shuile.Root;

namespace Shuile.Rhythm.Runtime
{
    public class BaseNoteData : IRhythmable
    {
        /// <summary> unit: in chart time </summary>
        public float rhythmTime;
        public virtual void Process() { }
        public virtual float ToPlayTime()
            => this.GetRealTime(rhythmTime, LevelRoot.LevelContext.timingManager);

        public static BaseNoteData Create(float time)
            => new() { rhythmTime = time };

        public override string ToString()
        {
            return $"{GetType().Name} targetTime: {rhythmTime}";
        }
    }

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
