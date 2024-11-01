using Cysharp.Threading.Tasks;
using Shuile.Core.Gameplay.Data;
using Shuile.Core.Global;
using Shuile.Core.Global.Config;
using Shuile.Framework;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Model;
using Shuile.Model;
using Shuile.Rhythm;
using System;
using UnityEngine;
using UObject = UnityEngine.Object;
using URandom = UnityEngine.Random;

namespace Shuile.Gameplay.Manager
{
    public class EnemySpawnManager
    {
        private readonly Transform _enemyParent;
        private readonly AutoPlayChartManager _autoPlayChartManager;
        private readonly LevelEntityManager _levelEntityManager;
        private readonly LevelModel _levelModel;
        private readonly LevelZoneManager _levelZoneManager;
        private readonly LevelEnemySO currentEnemyData;

        private readonly UnityEntryPointScheduler _scheduler;
        private PrefabConfigSO _globalPrefabs;

        public EnemySpawnManager(RuntimeContext context, Transform enemyParent)
        {
            context
                .Resolve(out _autoPlayChartManager)
                .Resolve(out _levelEntityManager)
                .Resolve(out _scheduler)
                .Resolve(out _levelZoneManager)
                .Resolve(out _levelModel)
                .Resolve(out SingleLevelData levelContext);

            _enemyParent = enemyParent;
            _globalPrefabs = GameApplication.BuiltInData.globalPrefabs;

            _scheduler.AddOnce(Start);
            _scheduler.AddCallOnDestroy(OnDestroy);

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
            if (currentEnemyCountIsTooLow) SpawnSingleEnemy();
        }

        private void SpawnSingleEnemy()
        {
            var (enemyType, pos) = RandomEnemyInfo();
            SpawnEnemyWithEffect(enemyType, pos).Forget();
        }

        public async UniTask SpawnEnemyWithEffect(EnemyType enemyType, Vector2 position)
        {
            var effect = _globalPrefabs.enemySpawnEffect;
            var effectInstance = UObject.Instantiate(effect.prefab);
            effectInstance.transform.position = position;

            var enemy = UObject.Instantiate(GetPrefabFromType(enemyType), position, Quaternion.identity, _enemyParent);
            enemy.SetActive(false);

            await UniTask.Delay(TimeSpan.FromSeconds(effect.duration),
                cancellationToken: _scheduler.destroyCancellationToken);

            enemy.SetActive(true);
        }

        private (EnemyType, Vector2) RandomEnemyInfo()
        {
            var dangerLevel = _levelModel.DangerLevel;
            var length = currentEnemyData.enemies.Length;
            var useIndex = dangerLevel <= length - 1 ? dangerLevel : length - 1;
            var useEnemies = currentEnemyData.enemies[useIndex].enemyList;
            var enemyType = useEnemies[URandom.Range(0, useEnemies.Length)];
            var pos = _levelZoneManager.RandomValidPosition();
            return (enemyType, pos);
        }

        private GameObject GetPrefabFromType(EnemyType enemyType)
        {
            var prefabConfig = _globalPrefabs;
            var res = enemyType switch
            {
                EnemyType.ZakoRobot => prefabConfig.zakoRobot,
                EnemyType.Creeper => prefabConfig.creeper,
                EnemyType.MahouDefenseTower => prefabConfig.mahouDefenseTower,
                _ => throw new Exception("Invalid EnemyType.")
            };
            return res;
        }
    }
}
