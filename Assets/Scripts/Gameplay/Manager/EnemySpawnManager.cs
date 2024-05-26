using CbUtils;
using CbUtils.Unity;
using Shuile.Core.Framework;
using Shuile.Core.Gameplay;
using Shuile.Gameplay;
using Shuile.Model;
using Shuile.Rhythm.Runtime;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace Shuile
{
    public class EnemySpawnManager : MonoSingletons<EnemySpawnManager>, IEntity
    {
        [HideInInspector] public LevelEnemySO currentEnemyData;

        private int currentRoundIndex = 0;
        private LevelModel levelModel;
        private AutoPlayChartManager _autoPlayChartManager;

        public int EnemyCount => LevelEntityManager.Instance.EnemyCount;
        public int CurrentRoundIndex => currentRoundIndex;

        protected override void OnAwake()
        {
            currentEnemyData = LevelDataBinder.Instance.levelEnemyData;
        }

        private void Start()
        {
            levelModel = this.GetModel<LevelModel>();
            _autoPlayChartManager = this.GetSystem<AutoPlayChartManager>();
            _autoPlayChartManager.OnRhythmHit += OnRhythmHit;
        }
        private void OnDestroy()
        {
            _autoPlayChartManager.OnRhythmHit -= OnRhythmHit;
        }
        private void OnRhythmHit()
        {
            var dangerLevel = levelModel.DangerLevel;
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
            LevelEntityFactory.Instance.SpawnEnemyWithEffectDelay(useEnemies[index], LevelZoneManager.Instance.RandomValidPosition());
        }

        public void OnSelfEnable()
        {
            throw new System.NotImplementedException();
        }

        public LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;
    }
}
