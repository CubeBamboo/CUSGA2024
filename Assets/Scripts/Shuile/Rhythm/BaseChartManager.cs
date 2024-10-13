using CbUtils.Extension;
using Shuile.Chart;
using Shuile.Core.Gameplay.Data;
using Shuile.Framework;
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

        public BaseChartManager(RuntimeContext context)
        {
            _entityManager = context.GetImplementation<LevelEntityManager>();
            _levelZoneManager = context.GetImplementation<LevelZoneManager>();
            _timingManager = context.GetImplementation<LevelTimingManager>();
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
                    throw new NotSupportedException();
                    break;
                case not null:
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
