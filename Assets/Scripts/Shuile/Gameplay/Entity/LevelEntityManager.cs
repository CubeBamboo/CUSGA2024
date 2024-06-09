using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Core.Global;
using Shuile.Core.Global.Config;
using Shuile.Gameplay.Event;
using Shuile.Model;
using Shuile.ResourcesManagement.Loader;
using Shuile.Rhythm.Runtime;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Shuile.Gameplay.Entity
{
    public class LevelEntityManager : IEntity, IStartable, IDestroyable
    {
        private readonly List<Enemy> _enemyList = new();

        private bool _judging = false;
        private int _frameCounter = 0;
        
        private Transform _enemyParent;

        private readonly LevelModel _levelModel;
        private readonly PrefabConfigSO _globalPrefab;

        internal LevelEntityFactory EntityFactory { get; private set; }

        public bool IsJudging => _judging;

        public Transform EnemyParent => _enemyParent;

        public int EnemyCount { get; set; }
        public ReadOnlyCollection<Enemy> Enemies => _enemyList.AsReadOnly();

        public LevelEntityManager(IGetableScope scope)
        {
            var resourceLoader = LevelResourcesLoader.Instance;
            _globalPrefab = resourceLoader.SyncContext.globalPrefabs;
            _levelModel = this.GetModel<LevelModel>();
        }

        public void Start()
        {
            EntityFactory = new LevelEntityFactory(this, _globalPrefab);
            
            _enemyParent = new GameObject("Enemies").transform;
            
            this.RegisterEvent<EnemySpawnEvent>(OnEnemySpawn);
            this.RegisterEvent<EnemyDieEvent>(OnEnemyDie);
        }

        public void OnDestroy()
        {
            this.UnRegisterEvent<EnemySpawnEvent>(OnEnemySpawn);
            this.UnRegisterEvent<EnemyDieEvent>(OnEnemyDie);
        }

        private void OnEnemySpawn(EnemySpawnEvent evt)
        {
            if (evt.enemy.TryGetComponent<Enemy>(out var enemy))
                MarkEnemy(enemy);
            EnemyCount++;
        }
        private void OnEnemyDie(EnemyDieEvent evt)
        {
            _levelModel.DangerScore += DangerLevelUtils.GetEnemyKillAddition();
            if (evt.enemy.TryGetComponent<Enemy>(out var enemy))
                _enemyList.UnorderedRemove(enemy);
            EnemyCount--;
        }

        public void OnRhythmHit()
        {
            _judging = true;
            var version = _frameCounter++;

            // common judgeable
            foreach (var judge in _levelModel.JudgeObjects)
            {
                judge.Judge(version, false);
            }

            // Judge enemy first
            foreach (var enemy in _enemyList)
                enemy.Judge(version, false);

            _judging = false;
        }
        
        public void RemoveImmediate(Enemy enemy)
        {
            _enemyList.UnorderedRemove(enemy);
        }

        public void MarkEnemy(Enemy enemy)
        {
            _enemyList.Add(enemy);
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
