using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Core.Gameplay.Data;
using Shuile.Core.Global.Config;
using Shuile.Gameplay.Entity;
using Shuile.Model;
using Shuile.Rhythm.Runtime;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace Shuile.Gameplay.Manager
{
    public class EnemySpawnManager : ISystem, IStartable, IDestroyable
    {
        private readonly LevelModel _levelModel;
        [HideInInspector] public readonly LevelEnemySO currentEnemyData;

        private readonly AutoPlayChartManager _autoPlayChartManager;
        private readonly LevelEntityManager _levelEntityManager;

        public int EnemyCount => _levelEntityManager.EnemyCount;

        public EnemySpawnManager(IGetableScope scope)
        {
            _autoPlayChartManager = scope.Get<AutoPlayChartManager>();
            _levelEntityManager = scope.Get<LevelEntityManager>();
            _levelModel = this.GetModel<LevelModel>();
            currentEnemyData = LevelRoot.LevelContext.LevelEnemyData;
        }

        public void Start()
        {
            _autoPlayChartManager.OnRhythmHit += OnRhythmHit;
        }
        
        public void OnDestroy()
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
            var useIndex = level <= length - 1 ? level : length-1;
            var useEnemies = currentEnemyData.enemies[useIndex].enemyList;
            int index = URandom.Range(0, useEnemies.Length);
            _levelEntityManager.EntityFactory.SpawnEnemyWithEffectDelay(useEnemies[index], LevelZoneManager.Instance.RandomValidPosition());
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}