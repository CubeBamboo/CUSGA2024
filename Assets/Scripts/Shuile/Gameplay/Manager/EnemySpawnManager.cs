using Shuile.Core.Gameplay.Data;
using Shuile.Core.Global;
using Shuile.Core.Global.Config;
using Shuile.Framework;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Model;
using Shuile.Model;
using Shuile.Rhythm;
using URandom = UnityEngine.Random;

namespace Shuile.Gameplay.Manager
{
    public class EnemySpawnManager
    {
        private readonly AutoPlayChartManager _autoPlayChartManager;
        private readonly LevelEntityManager _levelEntityManager;
        private readonly LevelModel _levelModel;
        private readonly LevelZoneManager _levelZoneManager;
        private readonly SceneTransitionManager _sceneTransitionManager;
        private readonly LevelEnemySO currentEnemyData;

        public EnemySpawnManager(RuntimeContext context)
        {
            context
                .Resolve(out _autoPlayChartManager)
                .Resolve(out _sceneTransitionManager)
                .Resolve(out _levelEntityManager)
                .Resolve(out LevelContext levelContext)
                .Resolve(out UnityEntryPointScheduler scheduler);

            _levelZoneManager = context.GetImplementation<LevelZoneManager>();
            _levelModel = context.GetImplementation<LevelModel>();

            scheduler.AddOnce(Start);
            scheduler.AddCallOnDestroy(OnDestroy);

            currentEnemyData = levelContext.LevelEnemyData;
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
                _levelZoneManager.RandomValidPosition(), _sceneTransitionManager.SceneChangedToken);
        }
    }
}
