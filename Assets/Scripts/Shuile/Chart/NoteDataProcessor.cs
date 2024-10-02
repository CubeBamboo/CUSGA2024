using CbUtils.Extension;
using Shuile.Core.Framework.Unity;
using Shuile.Core.Gameplay.Data;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Manager;
using Shuile.Rhythm;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Shuile.Chart
{
    [Obsolete]
    public class NoteDataProcessor
    {
        private readonly LevelEntityManager _entityManager;
        private readonly LevelZoneManager _levelZoneManager;
        private readonly LevelTimingManager _timingManager;

        public NoteDataProcessor(IGetableScope scope, LevelEntityManager entityManager)
        {
            _entityManager = entityManager;
            _levelZoneManager = scope.GetImplementation<LevelZoneManager>();
            _timingManager = scope.GetImplementation<LevelTimingManager>();
        }

        public void ProcessNote(BaseNoteData noteData)
        {
            switch (noteData)
            {
                case SpawnLaserNoteData laserNoteData:
                    _entityManager.EntityFactory.SpawnLaser()
                        .SetPosition(_levelZoneManager.RandomValidPosition());
                    break;
                case not null:
                    Debug.LogWarning("unknown note type");
                    break;
                default:
                    throw new ArgumentException();
            }
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
    }
}
