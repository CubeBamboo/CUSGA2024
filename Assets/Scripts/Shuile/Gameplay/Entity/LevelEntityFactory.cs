using CbUtils.Extension;
using CbUtils.Timing;
using Shuile.Core.Gameplay.Data;
using Shuile.Core.Global;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Shuile.Gameplay.Entity
{
    internal class LevelEntityFactory
    {
        public LevelEntityManager LevelEntityManager { get; private set; }
        public PrefabConfigSO PrefabConfig { get; private set; }

        public LevelEntityFactory(LevelEntityManager levelEntityManager, PrefabConfigSO prefabConfig)
        {
            LevelEntityManager = levelEntityManager;
            PrefabConfig = prefabConfig;
        }
        
        #region Enemy

        private GameObject InternalSpawnEnemy(GameObject enemyPrefab, Vector3 pos)
        {
            var enemyObject = UObject.Instantiate(enemyPrefab, pos, Quaternion.identity, LevelEntityManager.EnemyParent);
            return enemyObject;
        }

        public GameObject SpawnEnemy(GameObject enemyPrefab, Vector3 pos)
            => InternalSpawnEnemy(enemyPrefab, pos);
        public GameObject SpawnEnemy(EnemyType enemyType, Vector3 pos)
            => InternalSpawnEnemy(EnemyType2Prefab(enemyType), pos);
        public GameObject SpawnEnemy(EnemyType enemyType)
            => InternalSpawnEnemy(EnemyType2Prefab(enemyType), Vector3.zero);

        public GameObject SpawnEnemyWithEffectDelay(EnemyType enemyType, Vector3 pos)
        {
            var effect = PrefabConfig.enemySpawnEffect;
            effect.effect.Instantiate()
                  .SetPosition(pos);

            var enemy = InternalSpawnEnemy(EnemyType2Prefab(enemyType), pos);
            enemy.SetActive(false);
            TimingCtrl.Instance
                .Timer(effect.duration, () => enemy.SetActive(true))
                .Start();
            return enemy;
        }

        #endregion

        #region Mechanism

        public GameObject SpawnLaser()
        {
            var go = PrefabConfig.laser.Instantiate(); // spawn
            return go;
        }

        #endregion
        
        public UnityEngine.GameObject EnemyType2Prefab(EnemyType enemyType)
        {
            PrefabConfigSO prefabConfig = PrefabConfig;
            var res = enemyType switch
            {
                EnemyType.ZakoRobot => prefabConfig.zakoRobot,
                EnemyType.Creeper => prefabConfig.creeper,
                EnemyType.MahouDefenseTower => prefabConfig.mahouDefenseTower,
                _ => throw new System.Exception("Invalid EnemyType."),
            };
            return res;
        }
    }
}
