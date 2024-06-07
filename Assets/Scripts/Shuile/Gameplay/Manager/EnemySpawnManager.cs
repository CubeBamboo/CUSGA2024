using Shuile.Core.Framework;
using Shuile.Core.Gameplay;
using Shuile.Model;
using Shuile.Root;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace Shuile.Gameplay.Manager
{
    public class EnemySpawnManager : ISystem
    {
        private readonly LevelModel _levelModel;
        [HideInInspector] public LevelEnemySO currentEnemyData;

        private int currentRoundIndex = 0;

        public int EnemyCount => LevelEntityManager.Instance.EnemyCount;
        public int CurrentRoundIndex => currentRoundIndex;

        public EnemySpawnManager()
        {
            currentEnemyData = LevelRoot.LevelContext.levelEnemyData;
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

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
