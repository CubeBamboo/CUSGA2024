using CbUtils;
using CbUtils.Extension;
using CbUtils.ActionKit;
using Shuile.Root;
using Shuile.Gameplay;

using UnityEngine;
using UObject = UnityEngine.Object;
using Shuile.Gameplay.Event;
using Shuile.Gameplay.Entity;

namespace Shuile
{
    public class EntityFactory : CSharpLazySingletons<EntityFactory>
    {
        #region Enemy

        private GameObject InternalSpawnEnemy(GameObject enemyPrefab, Vector3 pos)
        {
            var enemyObject = UObject.Instantiate(enemyPrefab, pos, Quaternion.identity, EntityManager.Instance.EnemyParent);
            EnemySpawnEvent.Trigger(enemyObject);
            return enemyObject;
        }

        public GameObject SpawnEnemy(GameObject enemyPrefab, Vector3 pos)
            => InternalSpawnEnemy(enemyPrefab, pos);
        public GameObject SpawnEnemy(EnemyType enemyType, Vector3 pos)
            => InternalSpawnEnemy(EntityUtils.EnemyType2Prefab(enemyType), pos);
        public GameObject SpawnEnemy(EnemyType enemyType)
            => InternalSpawnEnemy(EntityUtils.EnemyType2Prefab(enemyType), Vector3.zero);

        public GameObject SpawnEnemyWithEffectDelay(EnemyType enemyType, Vector3 pos)
        {
            var effect = LevelResources.Instance.globalPrefabs.enemySpawnEffect;
            effect.effect.Instantiate()
                  .SetPosition(pos);

            var enemy = InternalSpawnEnemy(EntityUtils.EnemyType2Prefab(enemyType), pos);
            enemy.gameObject.SetActive(false);
            ActionCtrl.Delay(effect.duration)
                      .OnComplete(() => enemy.gameObject.SetActive(true))
                      .Start(LevelRoot.Instance.gameObject);
            return enemy;
        }

        #endregion

        public Gadget SpawnGadget(GameObject gadgetPrefab, float destroyTime, Vector3 pos, Vector3 rotation)
        {
            var gadgetObject = UObject.Instantiate(gadgetPrefab, pos, Quaternion.Euler(rotation));
            var gadget = gadgetObject.GetComponent<Gadget>();

            gadget.destroyTime = destroyTime;
            EntityManager.Instance.MarkGadget(gadget);
            return gadget;
        }
    }
}
