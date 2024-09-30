using CbUtils.Extension;
using Shuile.Chart;
using Shuile.Core.Framework.Unity;
using Shuile.Core.Gameplay.Data;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Manager;
using System;

namespace Shuile.Rhythm.Runtime
{
    public class BaseChartManager
    {
        private readonly LevelEntityManager _entityManager;
        private readonly LevelZoneManager _levelZoneManager;
        private readonly LevelTimingManager _timingManager;

        public BaseChartManager(IGetableScope scope)
        {
            _entityManager = scope.GetImplementation<LevelEntityManager>();
            _levelZoneManager = scope.GetImplementation<LevelZoneManager>();
            _timingManager = scope.GetImplementation<LevelTimingManager>();
        }

        public float GetNotePlayTime(BaseNoteData noteData)
        {
            return noteData switch
            {
                SpawnLaserNoteData laserNoteData => _timingManager.GetRealTime(noteData.rhythmTime - Laser.InTime),
                SpawnSingleEnemyNoteData enemyNoteData => _timingManager.GetRealTime(noteData.rhythmTime),
                not null => _timingManager.GetRealTime(noteData.rhythmTime),
                _ => throw new ArgumentException()
            };
        }

        public void ProcessNote(BaseNoteData noteData)
        {
            switch (noteData)
            {
                case SpawnLaserNoteData laserNoteData:
                    _entityManager.EntityFactory.SpawnLaser()
                        .SetPosition(_levelZoneManager.RandomValidPosition());
                    break;
                case SpawnSingleEnemyNoteData enemyNoteData:
                    var randomType = (EnemyType)UnityEngine.Random.Range(0, (int)EnemyType.TotalCount);
                    _entityManager.EntityFactory.SpawnEnemy(randomType, _levelZoneManager.RandomValidPosition());
                    break;
                case not null:
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
