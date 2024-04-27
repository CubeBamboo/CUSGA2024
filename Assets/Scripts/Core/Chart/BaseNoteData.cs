// [WIP]

using CbUtils.Extension;
using Shuile.Gameplay;
using Shuile.NoteProduct;
using UnityEngine;

namespace Shuile.Rhythm.Runtime
{
    public class BaseNoteData : IRhythmable
    {
        public float targetTime;
        public virtual void Process() { }
        public virtual float ToPlayTime()
            => this.GetRhythmTime(targetTime);

        public static BaseNoteData Create(float time)
            => new() { targetTime = time };

        public override string ToString()
        {
            return $"{GetType().Name} targetTime: {targetTime}";
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
            => this.GetRhythmTime(targetTime - Laser.InTime);
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
