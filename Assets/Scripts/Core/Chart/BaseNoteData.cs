// [WIP]

using UnityEngine;

using CbUtils.Extension;

namespace Shuile.Rhythm.Runtime
{
    public class BaseNoteData : IRhythmable
    {
        /// <summary> unit: in chart time </summary>
        public float rhythmTime;
        public virtual void Process() { }
        public virtual float ToPlayTime()
            => this.GetRealTime(rhythmTime);

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
            EntityFactory.Instance.SpawnLaser()
                .SetPosition(LevelZoneManager.Instance.RandomValidPosition());
        }

        public override float ToPlayTime()
            => this.GetRealTime(rhythmTime - Laser.InTime);
    }

    public class SpawnSingleEnemyNoteData : BaseNoteData
    {
        public override void Process()
        {
            var randomType = (EnemyType)Random.Range(0, (int)EnemyType.TotalCount);
            EntityFactory.Instance.SpawnEnemy(randomType, LevelZoneManager.Instance.RandomValidPosition());
        }
    }
}
