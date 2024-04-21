using CbUtils;
using CbUtils.ActionKit;

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

        private UniTask InternalLoadLevel(int index)// TODO: map index to loadkey
            => InternalLoadScene("Level0Test");
        

        public void ToLevelScene(int index)
        {
            this.DoTransition(async () => await InternalLoadLevel(index), defaultLoadingViewer);
        }

        public void ToMenu()
        {
            this.DoTransition(async () => await InternalLoadScene("MainMenu"), defaultLoadingViewer);
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
