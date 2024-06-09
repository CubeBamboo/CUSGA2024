using CbUtils.Extension;
using Shuile.Core.Framework.Unity;
using Shuile.Core.Gameplay.Data;
using Shuile.Gameplay;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Manager;
using System;
using Random = UnityEngine.Random;

namespace Shuile.Chart
{
    public class NoteDataProcessor
    {
        private readonly LevelEntityManager _entityManager;

        public NoteDataProcessor(IGetableScope scope)
        {
            _entityManager = scope.Get<LevelEntityManager>();
        }
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
                SpawnLaserNoteData laserNoteData => LevelRoot.LevelContext.TimingManager.GetRealTime(noteData.rhythmTime - Laser.InTime),
                SpawnSingleEnemyNoteData enemyNoteData => LevelRoot.LevelContext.TimingManager.GetRealTime(noteData.rhythmTime),
                not null => LevelRoot.LevelContext.TimingManager.GetRealTime(noteData.rhythmTime),
                _ => throw new ArgumentException()
            };
        }
    }
}
