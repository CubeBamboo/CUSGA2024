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
        private readonly Transform enemyParent;

        public LevelEntityFactory(LevelEntityManager levelEntityManager, PrefabConfigSO prefabConfig, Transform enemyParent)
        {
            this.enemyParent = enemyParent; // TODO: remove this
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
                UObject.Instantiate(enemyPrefab, pos, Quaternion.identity, enemyParent);
            return enemyObject;
        }

        public GameObject SpawnEnemy(EnemyType enemyType, Vector3 pos)
        {
            return InternalSpawnEnemy(EnemyType2Prefab(enemyType), pos);
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
