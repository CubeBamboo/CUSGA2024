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
        private LevelModel _levelModel;
        [HideInInspector] public LevelEnemySO currentEnemyData;

        private int currentRoundIndex = 0;
        private AutoPlayChartManager _autoPlayChartManager;
        private LevelEntityManager _levelEntityManager;

        public int EnemyCount => _levelEntityManager.EnemyCount;
        public int CurrentRoundIndex => currentRoundIndex;

        public void Start()
        {
            var lifeTimeLocator = LevelScope.Interface;
            _autoPlayChartManager = lifeTimeLocator.Get<AutoPlayChartManager>();
            _levelEntityManager = lifeTimeLocator.Get<LevelEntityManager>();
            currentEnemyData = LevelRoot.LevelContext.levelEnemyData;
            _levelModel = this.GetModel<LevelModel>();
            
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
