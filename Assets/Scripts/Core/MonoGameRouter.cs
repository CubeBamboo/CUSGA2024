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
        public string LastLevelSceneName { get; private set; }

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
            LastLevelSceneName = sceneName;
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            await UniTask.WaitUntil(() => !TaskBus.Instance.IsBusy);
            ExceptionUtils.UnityCatch(() =>
            {
                // maybe obsolete in future
                LevelStartEvent_AutoClear.Trigger(sceneName);
                LevelStartEvent.Trigger(sceneName);
                LevelLoadEndEvent.Trigger(sceneName); 
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
