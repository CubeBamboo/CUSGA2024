using CbUtils;

using System.Collections.Generic;

using UnityEngine;

namespace Shuile.Gameplay
{
    public class EnemyManager : MonoSingletons<EnemyManager>
    {
        [SerializeField] private PlayerController playerCtrl;
        private readonly List<Enemy> enemyList = new();
        private Transform enemyParent;
        private PrefabConfigSO prefabs;

        public PrefabConfigSO EnemyPrefabs
        {
            get
            {
                if (prefabs == null)
                    prefabs = GameplayService.Interface.Get<PrefabConfigSO>();
                return prefabs;
            }
        }

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
            foreach (var enemy in enemyList)
                enemy.Judge();
        }

        public void RemoveEnemy(Enemy enemy)
        {
            LevelGrid.Instance.grid.Remove(enemy.GridPosition);
            enemyList.UnorderedRemove(enemy);
            // Destroy(enemy.gameObject);
        }

        public void SpawnEnemy(GameObject enemyPrefab, Vector3Int pos)
        {
            if (LevelGrid.Instance.grid.IsOutOfBound(pos))
                return;

            var enemyObject = Instantiate(enemyPrefab, enemyParent);
            var enemy = enemyObject.GetComponent<Enemy>();
            enemy.GridPosition = pos;
            enemyList.Add(enemy);
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
