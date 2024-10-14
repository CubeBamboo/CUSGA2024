using CbUtils.Extension;
using Shuile.Gameplay.Entity.Enemies;
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
        }

        [UnityTest]
        public IEnumerator Test()
        {
            yield return null;
        }

        [UnityTest]
        public IEnumerator ZakoRobotAnim()
        {
            var instance = GameApplication.BuiltInData.globalPrefabs.zakoRobot.Instantiate();
            var zako = instance.GetComponent<ZakoMachine>();
            instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; // disable gravity

            yield return new WaitForSeconds(3f);
            zako.ForceDie();

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape) || Time.timeSinceLevelLoad > 180f);
        }
    }
}
