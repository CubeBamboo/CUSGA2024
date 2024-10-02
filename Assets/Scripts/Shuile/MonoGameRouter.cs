using CbUtils.Unity;
using Cysharp.Threading.Tasks;
using Shuile.Core.Global;
using Shuile.UI;
using UnityEngine.SceneManagement;

namespace Shuile
{
    public class MonoGameRouter : MonoSingletons<MonoGameRouter>, IGameRouter
    {
        private IRouterLoadingViewer defaultLoadingViewer;
        public string LastLevelSceneName { get; private set; }

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
            GameApplication.GlobalContext.Get<SceneTransitionManager>().OnNotifySceneChanged();
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

        public void ToMenu()
        {
            this.DoTransition(async () => await InternalLoadScene("MainMenu"), defaultLoadingViewer);
        }

        public Scene GetCurrentScene()
        {
            return SceneManager.GetActiveScene();
        }

        public void RestartGame()
        {
            SceneManager.LoadScene("Root");
        }

        public void LoadMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
