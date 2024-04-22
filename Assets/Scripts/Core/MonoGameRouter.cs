using CbUtils;

using Cysharp.Threading.Tasks;
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

        private async UniTask InternalLoadScene(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }

        private UniTask InternalLoadLevel(string sceneName)
            => InternalLoadScene(sceneName);
            //=> InternalLoadScene("Level0Test");


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
