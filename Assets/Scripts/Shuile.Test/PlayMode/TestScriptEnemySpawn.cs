using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Shuile.Core.Gameplay.Data;
using Shuile.Framework;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Manager;
using UnityEngine;

namespace Shuile.Test.PlayMode
{
    public class TestScriptEnemySpawn : RootTestSceneTestScript
    {
        [Test]
        public void Spawn()
        {
            var enemyPool = new EnemyPool(null);
            enemyPool.GetByEnum(EnemyType.ZakoRobot).GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; // disable gravity

            Do().Forget();

            UnityEntryPointScheduler.Create(new GameObject()).AddOnGUI(enemyPool.UnityOnGUI);

            return;

            async UniTaskVoid Do()
            {
                for (int i = 0; i < 15; i++)
                {
                    var enemy = enemyPool.GetByEnum(EnemyType.ZakoRobot);
                    enemy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; // disable gravity

                    enemy.DieFxEnd += enemyPool.Release;
                    enemy.transform.position = new Vector2(Random.Range(-10f, 10f), Random.Range(-5f, 5f));

                    EnemyDo(enemy).Forget();

                    await UniTask.Delay(1000);
                }
            }

            async UniTaskVoid EnemyDo(Enemy enemy)
            {
                await UniTask.Delay(Random.Range(200, 3000));
                enemy.ForceDie();
            }
        }

        private class TestZoneManager : ILevelZoneManager
        {
            public Vector2 RandomValidPosition()
            {
                return new Vector2(Random.Range(-10f, 10f), Random.Range(-5f, 5f));
            }
        }
    }
}
