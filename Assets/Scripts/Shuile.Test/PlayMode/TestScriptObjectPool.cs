using CbUtils.Extension;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Shuile.Chart;
using Shuile.Core.Gameplay.Data;
using Shuile.Framework;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Manager;
using Shuile.Model;
using Shuile.Rhythm;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile.Test.PlayMode
{
    public class TestScriptObjectPool : RootTestSceneTestScript
    {
        private List<Enemy> _enemies = new();

        [Test]
        public void SpawnEnemy()
        {
            var enemyPool = new EnemyPool(null);

            Do().Forget();

            var sc = UnityEntryPointScheduler.Create(new GameObject());
            sc.AddOnGUI(enemyPool.UnityOnGUI);

            return;

            async UniTaskVoid Do()
            {
                for (int i = 0; i < 15; i++)
                {
                    var enemy = enemyPool.GetByEnum(EnemyType.ZakoRobot);
                    enemy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; // disable gravity

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

            void InputUpdate()
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var byEnum = enemyPool.GetByEnum(EnemyType.ZakoRobot);
                    byEnum.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                    _enemies.Add(byEnum);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (_enemies.Count > 0)
                    {
                        Random.Range(0, _enemies.Count).With(i =>
                        {
                            var enemy = _enemies[i];
                            _enemies.RemoveAt(i);
                            enemyPool.KillEnemy(enemy);
                        });
                    }
                }
            }
        }

        [Test]
        public void SpawnLaser()
        {
            var factory = new LevelEntityFactory();
            new GameObject().AddComponent<TestSceneContainer>();

            var laser = GameApplication.BuiltInData.globalPrefabs.laser.Instantiate();
            laser.GetComponent<Laser>().Play();

            Do().Forget();

            async UniTaskVoid Do()
            {
                for (int i = 0; i < 15; i++)
                {
                    var laser = factory.SpawnLaser();
                    laser.transform.position = new Vector2(Random.Range(-10f, 10f), 0f);

                    await UniTask.Delay(Random.Range(50, 1500));
                }
            }
        }

        private class TestSceneContainer : SceneContainer
        {
            public override void BuildSelfContext(RuntimeContext context)
            {
                base.BuildSelfContext(context);
                context.RegisterInstance(new LevelTimingManager(new RuntimeContext()
                    .With(x =>
                    {
                        x.RegisterInstance(new LevelModel.TimingData(new[]
                        {
                            new ChartData.TimingPoint { bpm = 150, offset = 0 }
                        }));
                    })));
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
