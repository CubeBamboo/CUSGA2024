using CbUtils.Extension;
using CbUtils.Timing;
using Shuile.Core.Gameplay.Data;
using Shuile.Core.Global;
using Shuile.Utils;
using System;
using System.Threading;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Shuile.Gameplay.Entity
{
    internal class LevelEntityFactory
    {
        public LevelEntityFactory(LevelEntityManager levelEntityManager, PrefabConfigSO prefabConfig)
        {
            LevelEntityManager = levelEntityManager;
            PrefabConfig = prefabConfig;
        }

        public LevelEntityManager LevelEntityManager { get; }
        public PrefabConfigSO PrefabConfig { get; }

        #region Mechanism

        public GameObject SpawnLaser()
        {
            var go = PrefabConfig.laser.Instantiate(); // spawn
            return go;
        }

        #endregion

        public GameObject EnemyType2Prefab(EnemyType enemyType)
        {
            var prefabConfig = PrefabConfig;
            var res = enemyType switch
            {
                EnemyType.ZakoRobot => prefabConfig.zakoRobot,
                EnemyType.Creeper => prefabConfig.creeper,
                EnemyType.MahouDefenseTower => prefabConfig.mahouDefenseTower,
                _ => throw new Exception("Invalid EnemyType.")
            };
            return res;
        }

        #region Enemy

        private GameObject InternalSpawnEnemy(GameObject enemyPrefab, Vector3 pos)
        {
            var enemyObject =
                UObject.Instantiate(enemyPrefab, pos, Quaternion.identity, LevelEntityManager.EnemyParent);
            return enemyObject;
        }

        public GameObject SpawnEnemy(GameObject enemyPrefab, Vector3 pos)
        {
            return InternalSpawnEnemy(enemyPrefab, pos);
        }

        public GameObject SpawnEnemy(EnemyType enemyType, Vector3 pos)
        {
            return InternalSpawnEnemy(EnemyType2Prefab(enemyType), pos);
        }

        public GameObject SpawnEnemy(EnemyType enemyType)
        {
            return InternalSpawnEnemy(EnemyType2Prefab(enemyType), Vector3.zero);
        }

        public GameObject SpawnEnemyWithEffectDelay(EnemyType enemyType, Vector3 pos, CancellationToken token = default)
        {
            var effect = PrefabConfig.enemySpawnEffect;
            effect.effect.Instantiate()
                .SetPosition(pos);

            var enemy = InternalSpawnEnemy(EnemyType2Prefab(enemyType), pos);
            enemy.SetActive(false);

            UtilsCommands.SetTimer(effect.duration,
                () => enemy.SetActive(true),
                token);
            return enemy;
        }

        #endregion
    }
}
