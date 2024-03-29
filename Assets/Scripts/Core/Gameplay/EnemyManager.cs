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
            foreach (var enemy in enemyList)
                enemy.Judge();
            
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
            LevelGrid.Instance.grid.Remove(enemy.GridPosition);
            enemyList.UnorderedRemove(enemy);
        }

        public Enemy SpawnEnemy(GameObject enemyPrefab, Vector3Int pos)
        {
            // if (LevelGrid.Instance.grid.IsOutOfBound(pos))
            //     return null;

            var enemyObject = Instantiate(enemyPrefab, LevelGrid.Instance.grid.CellToWorld(pos), Quaternion.identity, enemyParent);
            var enemy = enemyObject.GetComponent<Enemy>();
            enemy.GridPosition = pos;
            enemyList.Add(enemy);
            return enemy;
        }

        // author: CubeBamboo
        public bool TryGetEnemyAtPosition(Vector3Int pos, out Enemy enemy)
        {
            enemy = null;
            if (LevelGrid.Instance.grid.IsValidCell(pos))
            {
                enemy = LevelGrid.instance.grid.contents[pos.x, pos.y].GetComponent<Enemy>();
                return enemy != null;
            }
            return false;
        }
        // end
    }
}
