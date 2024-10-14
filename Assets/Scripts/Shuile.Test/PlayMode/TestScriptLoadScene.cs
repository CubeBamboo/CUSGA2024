using Shuile.Core.Gameplay.Data;
using Shuile.Gameplay;
using Shuile.Gameplay.Model;
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

        [UnityTest]
        public IEnumerator ToTestLevel()
        {
            var levelData = GameApplication.BuiltInData.levelDataMap.FirstByLabel("Ginevra");
            var sceneMeta =
                new LevelSceneMeta(new LevelContext(levelData));
            MonoGameRouter.Instance.LoadScene(sceneMeta);
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape) || Time.timeSinceLevelLoad > 180f);
        }
    }
}
