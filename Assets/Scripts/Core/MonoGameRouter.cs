using CbUtils;
using CbUtils.Kits.Tasks;
using CbUtils.Unity;
using Cysharp.Threading.Tasks;
using Shuile.Gameplay.Event;
using Shuile.Utils;
using UnityEngine.SceneManagement;

namespace Shuile
{
    public class MonoGameRouter : MonoSingletons<MonoGameRouter>, IGameRouter
    {
        private IRouterLoadingViewer defaultLoadingViewer;

        protected override void OnAwake()
        {
            SetDontDestroyOnLoad();
            defaultLoadingViewer = GlobalTransitionViewer.Instance;
        }

        private async UniTask InternalLoadScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            await SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }

        private async UniTask InternalLoadLevel(string sceneName)
        {
            await InternalLoadScene("LevelDependency", LoadSceneMode.Single); // you can load resources you need in LevelDependency scene's GameObjects.
            await UniTask.WaitUntil(() => !TaskBus.Instance.IsBusy);
            await InternalLoadScene(sceneName, LoadSceneMode.Additive);
            ExceptionUtils.UnityCatch(() =>
            {
                LevelStartEvent_AutoClear.Trigger(sceneName);
                LevelStartEvent.Trigger(sceneName);
                LevelLoadEndEvent.Trigger(sceneName); // maybe obsolete in future
                LevelLoadEndEvent.Clear();
            });
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
            => SceneManager.GetActiveScene();
        
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
