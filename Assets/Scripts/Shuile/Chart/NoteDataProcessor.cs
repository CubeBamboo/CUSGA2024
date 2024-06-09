using CbUtils.Extension;
using Shuile.Core.Gameplay.Data;
using Shuile.Gameplay;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Manager;
using Shuile.Rhythm.Runtime;
using System;
using Random = UnityEngine.Random;

namespace Shuile.Chart
{
    public class NoteDataProcessor
    {
        private readonly LevelEntityManager _entityManager;

        public NoteDataProcessor(LevelEntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public void ProcessNote(BaseNoteData noteData)
        {
            switch (noteData)
            {
                case SpawnLaserNoteData laserNoteData:
                    _entityManager.EntityFactory.SpawnLaser()
                        .SetPosition(LevelZoneManager.Instance.RandomValidPosition());
                    break;
                case SpawnSingleEnemyNoteData enemyNoteData:
                    var randomType = (EnemyType)Random.Range(0, (int)EnemyType.TotalCount);
                    _entityManager.EntityFactory.SpawnEnemy(randomType, LevelZoneManager.Instance.RandomValidPosition());
                    break;
                case not null:
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public float GetNotePlayTime(BaseNoteData noteData)
        {
            return noteData switch
            {
                SpawnLaserNoteData laserNoteData => LevelRoot.LevelContext.timingManager.GetRealTime(noteData.rhythmTime - Laser.InTime),
                SpawnSingleEnemyNoteData enemyNoteData => LevelRoot.LevelContext.timingManager.GetRealTime(noteData.rhythmTime),
                not null => LevelRoot.LevelContext.timingManager.GetRealTime(noteData.rhythmTime),
                _ => throw new ArgumentException()
            };
        }
    }
}
