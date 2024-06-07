using CbUtils;
using CbUtils.Extension;
using CbUtils.Timing;
using Shuile.Core.Gameplay.Data;
using Shuile.Core.Global;
using Shuile.ResourcesManagement.Loader;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Shuile.Gameplay.Entity
{
    public class LevelEntityFactory : CSharpLazySingletons<LevelEntityFactory>
    {
        #region Enemy

        private GameObject InternalSpawnEnemy(GameObject enemyPrefab, Vector3 pos)
        {
            var enemyObject = UObject.Instantiate(enemyPrefab, pos, Quaternion.identity, LevelEntityManager.Instance.EnemyParent);
            return enemyObject;
        }

        public GameObject SpawnEnemy(GameObject enemyPrefab, Vector3 pos)
            => InternalSpawnEnemy(enemyPrefab, pos);
        public GameObject SpawnEnemy(EnemyType enemyType, Vector3 pos)
            => InternalSpawnEnemy(LevelEntityUtils.EnemyType2Prefab(enemyType), pos);
        public GameObject SpawnEnemy(EnemyType enemyType)
            => InternalSpawnEnemy(LevelEntityUtils.EnemyType2Prefab(enemyType), Vector3.zero);

        public GameObject SpawnEnemyWithEffectDelay(EnemyType enemyType, Vector3 pos)
        {
            var effect = LevelResourcesLoader.Instance.SyncContext.globalPrefabs.enemySpawnEffect;
            effect.effect.Instantiate()
                  .SetPosition(pos);

            var enemy = InternalSpawnEnemy(LevelEntityUtils.EnemyType2Prefab(enemyType), pos);
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
            PrefabConfigSO prefabConfig = LevelResourcesLoader.Instance.SyncContext.globalPrefabs;
            var go = prefabConfig.laser.Instantiate(); // spawn
            return go;
        }

        #endregion
    }
}
