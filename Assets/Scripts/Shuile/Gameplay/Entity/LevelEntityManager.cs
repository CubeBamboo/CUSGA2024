using CbUtils.Unity;
using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
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
        private readonly List<Enemy> enemyList = new();
        private readonly List<BehaviourLevelEntity> removeList = new();

        private bool judging = false;
        private int frameCounter = 0;
        
        private Transform _enemyParent;

        private LevelModel _levelModel;
        private AutoPlayChartManager _autoPlayChartManager;

        internal LevelEntityFactory EntityFactory { get; private set; }

        public bool IsJudging => judging;

        public Transform EnemyParent => _enemyParent;

        public int EnemyCount { get; set; }
        public ReadOnlyCollection<Enemy> Enemies => enemyList.AsReadOnly();

        public void Start()
        {
            var scope = LevelScope.Interface;
            var resourceLoader = LevelResourcesLoader.Instance;
            
            _enemyParent = new GameObject("Enemies").transform;
            _levelModel = this.GetModel<LevelModel>();
            
            _autoPlayChartManager = scope.Get<AutoPlayChartManager>();
            _autoPlayChartManager.OnRhythmHit += OnRhythmHit;

            this.RegisterEvent<EnemySpawnEvent>(OnEnemySpawn);
            this.RegisterEvent<EnemyDieEvent>(OnEnemyDie);

            EntityFactory = new LevelEntityFactory(this, resourceLoader.SyncContext.globalPrefabs);
        }

        public void OnDestroy()
        {
            this.UnRegisterEvent<EnemySpawnEvent>(OnEnemySpawn);
            this.UnRegisterEvent<EnemyDieEvent>(OnEnemyDie);

            _autoPlayChartManager.OnRhythmHit -= OnRhythmHit;
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
                enemyList.UnorderedRemove(enemy);
            EnemyCount--;
        }

        private void OnRhythmHit()
        {
            judging = true;
            var version = frameCounter++;

            // common judgeable
            foreach (var judge in _levelModel.JudgeObjects)
            {
                judge.Judge(version, false);
            }

            // Judge enemy first
            foreach (var enemy in enemyList)
                enemy.Judge(version, false);
            //foreach (Enemy enemy in removeList)
            //    RemoveImmediate(enemy);
            removeList.Clear();

            judging = false;
        }

        public void Remove<T>(T behaviourEntity) where T : BehaviourLevelEntity
        {
            if (judging)
            {
                removeList.Add(behaviourEntity);
                return;
            }
            RemoveImmediate(behaviourEntity);
        }

        public void RemoveImmediate<T>(T behaviourEntity) where T : BehaviourLevelEntity
        {
            if (behaviourEntity is Enemy enemy)
                enemyList.UnorderedRemove(enemy);
        }
        public void RemoveImmediate(Enemy enemy)
        {
            enemyList.UnorderedRemove(enemy);
        }

        public void MarkEnemy(Enemy enemy)
        {
            enemyList.Add(enemy);
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
