using Shuile.Core.Framework.Unity;
using Shuile.Core.Gameplay.Data;
using Shuile.Core.Global.Config;
using Shuile.Gameplay.Entity;
using Shuile.Model;
using Shuile.Rhythm;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace Shuile.Gameplay.Manager
{
    public class EnemySpawnManager : IStartable, IDestroyable
    {
        private readonly AutoPlayChartManager _autoPlayChartManager;
        private readonly LevelEntityManager _levelEntityManager;
        private readonly LevelModel _levelModel;
        [HideInInspector] public readonly LevelEnemySO currentEnemyData;
        private readonly LevelZoneManager _levelZoneManager;

        public EnemySpawnManager(IGetableScope scope)
        {
            _autoPlayChartManager = scope.GetImplementation<AutoPlayChartManager>();
            _levelEntityManager = scope.GetImplementation<LevelEntityManager>();
            _levelZoneManager = scope.GetImplementation<LevelZoneManager>();
            _levelModel = scope.GetImplementation<LevelModel>();
            currentEnemyData = LevelRoot.LevelContext.LevelEnemyData;
        }

        public int EnemyCount => _levelEntityManager.EnemyCount;

        public void OnDestroy()
        {
            _autoPlayChartManager.OnRhythmHit += OnRhythmHit;
        }

        public void Start()
        {
            _autoPlayChartManager.OnRhythmHit += OnRhythmHit;
        }

        public void OnRhythmHit()
        {
            var dangerLevel = _levelModel.DangerLevel;
            var currentEnemyCountIsTooLow = EnemyCount <= DangerLevelUtils.GetEnemySpawnThreshold(dangerLevel);
            //var currentEnemyCountIsTooLow = EnemyCount <= 0;
            if (currentEnemyCountIsTooLow)
            {
                SpawnSingleEnemy(dangerLevel);
            }
        }

        private void SpawnSingleEnemy(int level)
        {
            var length = currentEnemyData.enemies.Length;
            var useIndex = level <= length - 1 ? level : length - 1;
            var useEnemies = currentEnemyData.enemies[useIndex].enemyList;
            var index = URandom.Range(0, useEnemies.Length);
            _levelEntityManager.EntityFactory.SpawnEnemyWithEffectDelay(useEnemies[index],
                _levelZoneManager.RandomValidPosition());
        }
    }
}
