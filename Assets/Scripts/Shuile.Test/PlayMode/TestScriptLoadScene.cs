using CbUtils.Extension;
using NUnit.Framework;
using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.Gameplay.Model;
using Shuile.UI;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Shuile.Test.PlayMode
{
    public class TestScriptLoadScene
    {
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/Scenes/Test/TestRootScene.unity",
                new LoadSceneParameters());
        }

        [Test]
        public void TestRootScene()
        {
            Assert.AreEqual(SceneManager.GetActiveScene().name, "TestRootScene");
        }

        [UnityTest]
        public IEnumerator ToTestLevel()
        {
            var levelData = GameApplication.BuiltInData.levelDataMap.FirstByLabel("Ginevra");
            var sceneMeta =
                new LevelSceneMeta(new SingleLevelData(levelData));
            MonoGameRouter.Instance.LoadScene(sceneMeta);
            yield return null;
        }

        [Test]
        public void LoadEndStaticsPanel()
        {
            var load = Resources.Load<GameObject>("UIDesign/EndStaticsPanel");

            var context = new RuntimeContext();
            context.RegisterInstance(new EndStaticsPanel.Data
            {
                SongName = "Waht can i say?",
                Composer = "LaoDa 劳大",
                HealthLoss = 1152,
                HitOnRhythm = 441,
                Score = 185632,
                TotalHit = 156,
                TotalKillEnemy = 77
            });

            using (MonoContainer.EnqueueParent(context))
            {
                load.Instantiate().SetParent(UIHelper.Root.transform, false);
            }
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape) || Time.timeSinceLevelLoad > 180f);
        }
    }
}
