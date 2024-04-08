using CbUtils;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;

namespace Shuile.Gameplay
{
    public class EnemyManager : MonoSingletons<EnemyManager>
    {
        [SerializeField] private PlayerController playerCtrl;
        private readonly List<Enemy> enemyList = new();
        private Transform enemyParent;
        private PrefabConfigSO prefabs;
        private bool judging = false;
        private readonly List<Enemy> removeList = new();
        private int frameCounter = 0;

        public PrefabConfigSO EnemyPrefabs
        {
            get
            {
                if (prefabs == null)
                    prefabs = GameplayService.Interface.Get<PrefabConfigSO>();
                return prefabs;
            }
        }
        public ReadOnlyCollection<Enemy> Enemies => enemyList.AsReadOnly();

        protected override void Awake()
        {
            base.Awake();
            enemyParent = new GameObject("Enemies").transform;
        }

        private void OnEnable()
        {
            AutoPlayChartManager.Instance.OnRhythmHit += OnRhythmHit;
        }

        private void OnDisable()
        {
            AutoPlayChartManager.Instance.OnRhythmHit -= OnRhythmHit;
        }

        private void OnRhythmHit()
        {
            judging = true;
            var version = frameCounter++;
            foreach (var enemy in enemyList)
                enemy.Judge(version, false);
            
            judging = false;
            foreach (var enemy in removeList)
                RemoveEnemyImmediate(enemy);
            removeList.Clear();
        }

        public void RemoveEnemy(Enemy enemy)
        {
            if (judging)
            {
                removeList.Add(enemy);
                return;
            }
            RemoveEnemyImmediate(enemy);
        }

        private void RemoveEnemyImmediate(Enemy enemy)
        {
            enemyList.UnorderedRemove(enemy);
        }

        public Enemy SpawnEnemy(GameObject enemyPrefab, Vector3 pos)
        {
            // if (LevelGrid.Instance.grid.IsOutOfBound(pos))
            //     return null;

            var enemyObject = Instantiate(enemyPrefab, pos, Quaternion.identity, enemyParent);
            var enemy = enemyObject.GetComponent<Enemy>();
            enemyList.Add(enemy);
            return enemy;
        }
    }
}
