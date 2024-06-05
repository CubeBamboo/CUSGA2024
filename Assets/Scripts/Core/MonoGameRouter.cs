using CbUtils;
using CbUtils.Kits.Tasks;
using CbUtils.Unity;
using Cysharp.Threading.Tasks;
using Shuile.Gameplay.Event;
using Shuile.Utils;
using UnityEngine.SceneManagement;

namespace Shuile
{
    /* TODO-List:
     * - implement such two way of manage scene: 
     * - - 1. scene as a independent module (single load)
     * - - 2. scene as a child root of other scene. (register dependency relationship and auto load when using)
     */
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
            await SceneManager.LoadSceneAsync("LevelRoot", LoadSceneMode.Single);
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            await SceneManager.LoadSceneAsync("LevelChild", LoadSceneMode.Additive); // you can load resources you need in LevelChild scene's GameObjects.
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
