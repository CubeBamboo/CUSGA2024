using CbUtils.Unity;
using Cysharp.Threading.Tasks;
using Shuile.Framework;
using Shuile.UI;
using UnityEngine.SceneManagement;

namespace Shuile
{
    /// <summary>
    /// previously it's used to behave the loading view on loading scene.
    /// recently i want to use it to manage (take over) the scene/prefab load process.
    /// </summary>
    public class MonoGameRouter : MonoSingletons<MonoGameRouter>, IGameRouter
    {
        private IRouterLoadingViewer defaultLoadingViewer;
        public string LastLevelSceneName { get; private set; }
        public SceneMeta CurrentScene { get; private set; }

        protected override void OnAwake()
        {
            SetDontDestroyOnLoad();
            defaultLoadingViewer = GlobalTransitionViewer.Instance;
        }

        private static async UniTask InternalLoadScene(string sceneName,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            await SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }

        private async UniTask InternalLoadLevel(string sceneName)
        {
            LastLevelSceneName = sceneName;
            await InternalLoadScene(sceneName);
        }

        public void ToLevelScene(string sceneName)
        {
            this.DoTransition(async () => await InternalLoadLevel(sceneName), defaultLoadingViewer);
        }

        public void LoadFromName(string sceneName)
        {
            this.DoTransition(async () => await InternalLoadScene(sceneName), defaultLoadingViewer);
        }

        public void LoadScene(SceneMeta meta)
        {
            CurrentScene = meta;
            this.DoTransition(async () =>
            {
                using (MonoContainer.EnqueueParentForTop(meta.SceneContext))
                {
                    await InternalLoadScene(meta.SceneName);
                }
            }, defaultLoadingViewer);
        }

        public abstract class SceneMeta
        {
            public string SceneName { get; set; }
            // public LoadSceneMode LoadSceneMode { get; set; } // only use single mode

            public RuntimeContext SceneContext { get; set; } = new(); // will be added to the SceneContainer

            // now you have to initialize the name because we use it as a key to locate the scene.
            protected SceneMeta(string sceneName)
            {
                SceneName = sceneName;
            }
        }
    }
}
