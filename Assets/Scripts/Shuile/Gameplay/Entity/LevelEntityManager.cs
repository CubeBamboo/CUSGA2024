using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Core.Global;
using Shuile.Core.Global.Config;
using Shuile.Framework;
using Shuile.Gameplay.Event;
using Shuile.Model;
using Shuile.ResourcesManagement.Loader;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Shuile.Gameplay.Entity
{
    public class LevelEntityManager : IStartable, IDestroyable
    {
        private readonly Transform enemyParent;
        private readonly List<Enemy> _enemyList = new();
        private readonly PrefabConfigSO _globalPrefab;

        private readonly LevelModel _levelModel;

        private int _frameCounter;

        public LevelEntityManager(IReadOnlyServiceLocator serviceLocator, Transform enemyParent)
        {
            this.enemyParent = enemyParent;
            serviceLocator
                .Resolve(out _levelModel)
                .Resolve(out UnityEntryPointScheduler scheduler);

            var resourceLoader = LevelResourcesLoader.Instance;
            _globalPrefab = resourceLoader.SyncContext.globalPrefabs;

            scheduler.AddOnce(Start);
            scheduler.AddCallOnDestroy(OnDestroy);
        }

        internal LevelEntityFactory EntityFactory { get; private set; }

        public bool IsJudging { get; private set; }

        public Transform EnemyParent { get; private set; }

        public int EnemyCount { get; set; }
        public ReadOnlyCollection<Enemy> Enemies => _enemyList.AsReadOnly();

        public void OnDestroy()
        {
            TypeEventSystem.Global.UnRegister<EnemySpawnEvent>(OnEnemySpawn);
            TypeEventSystem.Global.UnRegister<EnemyDieEvent>(OnEnemyDie);
        }

        public void Start()
        {
            EntityFactory = new LevelEntityFactory(this, _globalPrefab, enemyParent);

            EnemyParent = new GameObject("Enemies").transform;

            TypeEventSystem.Global.Register<EnemySpawnEvent>(OnEnemySpawn);
            TypeEventSystem.Global.Register<EnemyDieEvent>(OnEnemyDie);
        }

        private void OnEnemySpawn(EnemySpawnEvent evt)
        {
            if (evt.enemy != null)
            {
                MarkEnemy(evt.enemy);
                EnemyCount++;
            }
            else
            {
                Debug.LogError("EnemySpawnEvent enemy is null.");
            }
        }

        private void OnEnemyDie(EnemyDieEvent evt)
        {
            _levelModel.DangerScore += DangerLevelUtils.GetEnemyKillAddition();
            if (evt.enemy.TryGetComponent<Enemy>(out var enemy))
            {
                _enemyList.UnorderedRemove(enemy);
            }

            EnemyCount--;
        }

        public void OnRhythmHit()
        {
            IsJudging = true;
            var version = _frameCounter++;

            // common judgeable
            foreach (var judge in _levelModel.JudgeObjects)
            {
                judge.Judge(version, false);
            }

            // Judge enemy first
            foreach (var enemy in _enemyList)
            {
                enemy.Judge(version, false);
            }

            IsJudging = false;
        }

        public void RemoveImmediate(Enemy enemy)
        {
            _enemyList.UnorderedRemove(enemy);
        }

        public void MarkEnemy(Enemy enemy)
        {
            _enemyList.Add(enemy);
        }
    }
}
