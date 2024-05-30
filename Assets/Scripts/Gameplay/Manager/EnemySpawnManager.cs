using Shuile.Core.Framework;
using Shuile.Core.Gameplay;
using Shuile.Gameplay;
using Shuile.Model;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace Shuile
{
    public class EnemySpawnManager : ISystem
    {
        [HideInInspector] public LevelEnemySO currentEnemyData;

        private int currentRoundIndex = 0;
        private LevelModel _levelModel;

        public int EnemyCount => LevelEntityManager.Instance.EnemyCount;
        public int CurrentRoundIndex => currentRoundIndex;

        public EnemySpawnManager()
        {
            currentEnemyData = LevelDataBinder.Instance.levelEnemyData;
            _levelModel = this.GetModel<LevelModel>();
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
            LevelEntityFactory.Instance.SpawnEnemyWithEffectDelay(useEnemies[index], LevelZoneManager.Instance.RandomValidPosition());
        }

        public LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;
    }
}
