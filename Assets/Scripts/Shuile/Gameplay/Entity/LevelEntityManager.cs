using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Core.Global;
using Shuile.Core.Global.Config;
using Shuile.Framework;
using Shuile.Gameplay.Event;
using Shuile.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Shuile.Gameplay.Entity
{
    public class LevelEntityManager : IStartable, IDestroyable
    {
        private readonly List<Enemy> _enemyList = new();
        private readonly PrefabConfigSO _globalPrefab;

        private readonly LevelModel _levelModel;

        private int _frameCounter;
        private Queue<Enemy> _removeQueue = new();

        public LevelEntityManager(IReadOnlyServiceLocator serviceLocator)
        {
            serviceLocator
                .Resolve(out _levelModel)
                .Resolve(out UnityEntryPointScheduler scheduler);

            _globalPrefab = GameApplication.BuiltInData.globalPrefabs;

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
            EntityFactory = new LevelEntityFactory(this, _globalPrefab);

            EnemyParent = new GameObject("Enemies").transform;

            // use global event system to handle enemy spawn and die, enemy will not focus on the outer metadata.
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
            QueueRemove(evt.enemy);

            EnemyCount--;
        }

        /// <summary>
        /// in case of removing enemy in the middle of the judgement (while collection is enumerating)
        /// </summary>
        /// <param name="enemy"></param>
        public void QueueRemove(Enemy enemy)
        {
            _removeQueue.Enqueue(enemy);
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

            // remove enemies
            while (_removeQueue.Count > 0)
            {
                var enemy = _removeQueue.Dequeue();
                RemoveImmediate(enemy);
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
