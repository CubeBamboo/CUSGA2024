using CbUtils;
using CbUtils.ActionKit;
using Shuile.Gameplay;

using UnityEngine;
using UObject = UnityEngine.Object;

namespace Shuile
{
    public class EntityFactory : CSharpLazySingletons<EntityFactory>
    {
        #region Enemy

        // TODO: support generic
        private Enemy InternalSpawnEnemy(GameObject enemyPrefab, Vector3 pos)
        {
            var enemyObject = UObject.Instantiate(enemyPrefab, pos, Quaternion.identity, EntityManager.Instance.EnemyParent);
            var enemy = enemyObject.GetComponent<Enemy>();
            EntityManager.Instance.MarkEnemy(enemy);
            return enemy;
        }

        public Enemy SpawnEnemy(GameObject enemyPrefab, Vector3 pos)
            => InternalSpawnEnemy(enemyPrefab, pos);
        public Enemy SpawnEnemy(EnemyType enemyType, Vector3 pos)
            => InternalSpawnEnemy(EntityUtils.EnemyType2Prefab(enemyType), pos);
        public Enemy SpawnEnemy(EnemyType enemyType)
            => InternalSpawnEnemy(EntityUtils.EnemyType2Prefab(enemyType), Vector3.zero);

        public Enemy SpawnEnemyWithEffectDelay(EnemyType enemyType, Vector3 pos)
        {
            var effect = LevelResources.Instance.globalPrefabs.enemySpawnEffect;
            effect.effect.Instantiate()
                  .SetPosition(pos);

            var enemy = InternalSpawnEnemy(EntityUtils.EnemyType2Prefab(enemyType), pos);
            enemy.gameObject.SetActive(false);
            ActionCtrl.Instance.Delay(effect.duration)
                      .OnComplete(() => enemy.gameObject.SetActive(true))
                      .Start();
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
