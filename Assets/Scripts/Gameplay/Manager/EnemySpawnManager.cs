using CbUtils;
using Shuile.Gameplay;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace Shuile
{
    public class EnemySpawnManager : MonoNonAutoSpawnSingletons<EnemySpawnManager>
    {
        [HideInInspector] public LevelEnemySO currentEnemyData;

        private int currentDangerLevel;

        private int currentRoundIndex = 0;
        private LevelModel levelModel;

        public int EnemyCount => EntityManager.Instance.Enemies.Count;
        public int CurrentRoundIndex => currentRoundIndex;

        protected override void OnAwake()
        {
            currentEnemyData = LevelDataBinder.Instance.levelEnemyData;
        }

        private void Start()
        {
            levelModel = GameplayService.Interface.Get<LevelModel>();
            levelModel.onRhythmHit += OnRhythmHit;
        }
        private void OnDestroy()
        {
            levelModel.onRhythmHit -= OnRhythmHit;
        }
        private void OnRhythmHit()
        {
            var dangerLevel = levelModel.DangerLevel;
            var currentEnemyCountIsTooLow = EnemyCount <= DangerLevelUtils.GetEnemySpawnCount(dangerLevel);
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
            EntityFactory.Instance.SpawnEnemyWithEffectDelay(useEnemies[index], LevelZoneManager.Instance.RandomValidPosition());
        }
    }
}
