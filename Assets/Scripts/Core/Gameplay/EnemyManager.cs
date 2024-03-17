using CbUtils;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

using UnityEngine;

namespace Shuile.Gameplay
{
    public class EnemyManager : MonoSingletons<EnemyManager>
    {
        private List<Enemy>[] enemyPosition;
        [SerializeField] private PlayerController playerCtrl;
        [SerializeField] private GameObject[] enemyPrefabs;
        private Transform enemyParent;

        public ReadOnlyCollection<GameObject> EnemyPrefabs { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            EnemyPrefabs = new(enemyPrefabs);
            enemyParent = new GameObject("Enemies").transform;
            enemyPosition = new List<Enemy>[10];  // 根据地图设计修改
            for (int i = 0; i < enemyPosition.Length; i++)
                enemyPosition[i] = new();
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
            var playerPos = Mathf.RoundToInt(playerCtrl.transform.position.x);  // Todo: 通过其他方式获取位置
            if (!IsValidPosition(playerPos))
                return;

            // allocate and sort;
            var judgeEnemies = new List<Enemy>(enemyPosition.Sum(l => l.Count));
            judgeEnemies.AddRange(enemyPosition[playerPos]);
            for (int i = 1; i < enemyPosition.Length; i++)
            {
                if (playerPos - i >= 0)
                    judgeEnemies.AddRange(enemyPosition[playerPos - i]);
                if (playerPos + i < enemyPosition.Length)
                    judgeEnemies.AddRange(enemyPosition[playerPos + i]);
            }

            foreach (var enemy in judgeEnemies)
                enemy.JudgeUpdate();
        }

        public void RemoveEnemy(Enemy enemy)
        {
            foreach (var e in enemyPosition)
            {
                var index = e.IndexOf(enemy);
                if (index == -1)
                    continue;

                e.UnorderedRemoveAt(index);
                break;
            }
        }

        public void SpawnEnemy(GameObject enemyPrefab, int pos)
        {
            if (!IsValidPosition(pos))
                return;

            var enemyObject = Instantiate(enemyPrefab, enemyParent);
            var enemy = enemyObject.GetComponent<Enemy>();
            enemy.Position = pos;
        }

        public void UpdateEnemyPosition(Enemy enemy, int pos)
        {
            if (!IsValidPosition(pos))
                return;

            enemyPosition[enemy.Position].UnorderedRemove(enemy);
            Debug.Log($"[{nameof(EnemyManager)}] Enemy {enemy.name} move from {enemy.Position} to {pos}");
            enemyPosition[pos].Add(enemy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidPosition(int pos) => pos >= 0 && pos < enemyPosition.Length;
    }
}
