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
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UObject = UnityEngine.Object;
using URandom = UnityEngine.Random;

namespace Shuile.Gameplay.Manager
{
    public class EnemySpawnManager
    {
        private readonly AutoPlayChartManager _autoPlayChartManager;
        private readonly LevelEntityManager _levelEntityManager;
        private readonly LevelModel _levelModel;

        private readonly UnityEntryPointScheduler _scheduler;

        private readonly EnemyInfoProcessor _infoProcessor;
        private EnemyPool _pool;

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

            _pool = new EnemyPool(enemyParent);

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
            var enemy = GetFromPool(enemyType).gameObject;
            enemy.transform.position = position;
            enemy.SetActive(false);

            await _infoProcessor.PlaySpawnEffect(position, _scheduler.destroyCancellationToken);

            enemy.SetActive(true);
        }

        private Enemy GetFromPool(EnemyType enemyType)
        {
            var enemy = _pool.GetByEnum(enemyType);
            enemy.DieFxEnd += _pool.Release;
            return enemy;
        }
    }

    public class EnemyPool
    {
        private readonly Transform _enemyParent;
        private Dictionary<EnemyType, ClassedObjectPool<Enemy>> _pools = new();

        public EnemyPool(Transform enemyParent)
        {
            _enemyParent = enemyParent;
            foreach (var enemyType in Enum.GetValues(typeof(EnemyType)))
            {
                var type = (EnemyType)enemyType;

                _pools.Add(type, new ClassedObjectPool<Enemy>(Create));
                continue;

                Enemy Create()
                {
                    var prefabConfigSo = GameApplication.BuiltInData.globalPrefabs;
                    return UObject.Instantiate(prefabConfigSo.GetPrefabFromType(type),
                            _enemyParent)
                        .GetComponent<Enemy>();
                }
            }
        }

        public Enemy GetByEnum(EnemyType enemyType)
        {
            return _pools[enemyType].Get();
        }

        public void Release(Enemy enemy)
        {
            _pools[enemy.CurrentType].Release(enemy);
        }

        public void UnityOnGUI()
        {
            GUI.skin.label.fontSize = 20;
            GUI.skin.label.padding = new RectOffset(10, 10, 10, 10);
            GUILayout.Label("Enemy Pool: " + _pools.Count);

            foreach (var (key, pool) in _pools)
            {
                GUILayout.Label(key + ": " + pool.Count);
                foreach (var obj in pool.EnumerateAll())
                {
                    GUILayout.Label(
                        UseHtmlText(obj.Obj.GetHashCode().ToString(),
                        obj.IsActive ? Color.yellow : Color.yellow.WithAlpha(0.4f)));
                }
            }

            return;

            string UseHtmlText(string text, Color color)
            {
                return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{text}</color>";
            }
        }
    }

    public class EnemyInfoProcessor
    {
        private readonly PrefabConfigSO _globalPrefabs = GameApplication.BuiltInData.globalPrefabs;

        private readonly LevelModel _levelModel;
        private readonly ILevelZoneManager _levelZoneManager;

        private readonly LevelEnemySO currentEnemyData;

        public EnemyInfoProcessor(IReadOnlyServiceLocator locator)
        {
            locator
                .Resolve(out currentEnemyData)
                .Resolve(out _levelModel)
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

        public UniTask PlaySpawnEffect(Vector2 position, CancellationToken cancellationToken)
        {
            var effect = _globalPrefabs.enemySpawnEffect;
            var effectInstance = UObject.Instantiate(effect.prefab);
            effectInstance.transform.position = position;

            return UniTask.Delay(TimeSpan.FromSeconds(effect.duration), cancellationToken: cancellationToken);
        }
    }
}
