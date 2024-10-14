using CbUtils.Extension;
using NUnit.Framework;
using Shuile.Gameplay.Entity;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Shuile.Test.PlayMode
{
    public class TestScriptEnemyAnimation
    {
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/Scenes/Test/TestRootScene.unity",
                new LoadSceneParameters());

            yield return new WaitForSeconds(0.5f);
        }

        [Test]
        public void SpawnEffect()
        {
            GameApplication.BuiltInData.globalPrefabs.enemySpawnEffect.prefab.Instantiate();
        }

        [UnityTest]
        public IEnumerator BombAnim()
        {
            var globalPrefabs = GameApplication.BuiltInData.globalPrefabs;
            var bomb = Object.Instantiate(globalPrefabs.mahouBomb, Vector3.zero, Quaternion.identity).GetComponent<Bomb>();

            yield return new WaitForSeconds(3f);

            bomb.Explode(100, 2f);
        }

        [UnityTest]
        public IEnumerator ZakoRobotAnim()
        {
            var instance = GameApplication.BuiltInData.globalPrefabs.zakoRobot.Instantiate();
            var enemy = instance.GetComponent<Enemy>();
            instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; // disable gravity

            yield return new WaitForSeconds(3f);
            enemy.ForceDie();

            yield return null;
        }

        [UnityTest]
        public IEnumerator CreeperAnim()
        {
            var instance = GameApplication.BuiltInData.globalPrefabs.creeper.Instantiate();
            var enemy = instance.GetComponent<Enemy>();
            instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; // disable gravity

            yield return new WaitForSeconds(3f);
            enemy.ForceDie();

            yield return null;
        }

        [UnityTest]
        public IEnumerator MahouTowerAnim()
        {
            var instance = GameApplication.BuiltInData.globalPrefabs.mahouDefenseTower.Instantiate();
            var enemy = instance.GetComponent<Enemy>();
            instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; // disable gravity

            yield return new WaitForSeconds(3f);
            enemy.ForceDie();

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape) || Time.timeSinceLevelLoad > 180f);
        }
    }
}
