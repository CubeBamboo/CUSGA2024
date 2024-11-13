using CbUtils.Extension;
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
using System.Threading;
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

        private readonly UnityEntryPointScheduler _scheduler;

        private EnemyInfoProcessor _infoProcessor;

        public EnemySpawnManager(RuntimeContext context, Transform enemyParent)
        {
            context
                .Resolve(out _autoPlayChartManager)
                .Resolve(out _levelEntityManager)
                .Resolve(out _scheduler)
                .Resolve(out _levelModel)
                .Resolve(out SingleLevelData levelContext);

            var currentEnemyData = levelContext.LevelEnemyData;

            _infoProcessor = new EnemyInfoProcessor(new RuntimeContext().With(x =>
            {
                x.AddParent(context);
                x.RegisterInstance(currentEnemyData);
            }));

            _enemyParent = enemyParent;

            _scheduler.AddOnce(Start);
            _scheduler.AddCallOnDestroy(OnDestroy);
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
            var (enemyType, pos) = _infoProcessor.RandomEnemyInfo();
            SpawnEnemyWithEffect(enemyType, pos).Forget();
        }

        public async UniTask SpawnEnemyWithEffect(EnemyType enemyType, Vector2 position)
        {
            var infoProcessor = _infoProcessor;

            var enemy = UObject.Instantiate(infoProcessor.GetPrefabFromType(enemyType), position, Quaternion.identity, _enemyParent);
            enemy.SetActive(false);

            await infoProcessor.PlaySpawnEffect(position, _scheduler.destroyCancellationToken);

            enemy.SetActive(true);
        }
    }

    internal class EnemyInfoProcessor
    {
        private readonly PrefabConfigSO _globalPrefabs = GameApplication.BuiltInData.globalPrefabs;

        private readonly LevelModel _levelModel;
        private readonly LevelZoneManager _levelZoneManager;

        private readonly LevelEnemySO currentEnemyData;

        public EnemyInfoProcessor(IReadOnlyServiceLocator locator)
        {
            locator
                .Resolve(out _levelModel)
                .Resolve(out currentEnemyData)
                .Resolve(out _levelZoneManager);
        }

        public (EnemyType, Vector2) RandomEnemyInfo()
        {
            var dangerLevel = _levelModel.DangerLevel;
            var length = currentEnemyData.enemies.Length;
            var useIndex = dangerLevel <= length - 1 ? dangerLevel : length - 1;
            var useEnemies = currentEnemyData.enemies[useIndex].enemyList;
            var enemyType = useEnemies[URandom.Range(0, useEnemies.Length)];
            var pos = _levelZoneManager.RandomValidPosition();
            return (enemyType, pos);
        }

        public GameObject GetPrefabFromType(EnemyType enemyType)
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

        public UniTask PlaySpawnEffect(Vector2 position, CancellationToken cancellationToken)
        {
            var effect = _globalPrefabs.enemySpawnEffect;
            var effectInstance = UObject.Instantiate(effect.prefab);
            effectInstance.transform.position = position;

            return UniTask.Delay(TimeSpan.FromSeconds(effect.duration), cancellationToken: cancellationToken);
        }
    }
}
