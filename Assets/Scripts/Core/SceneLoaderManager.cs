using UnityEngine.SceneManagement;

namespace Shuile.Framework
{
    public class SceneLoaderManager : ISceneLoader
    {
        public void LoadSceneAsync(SceneInfo sceneInfo, LoadSceneMode loadMode = LoadSceneMode.Single, bool showLoadingPanel = false, bool fadeInOut = true)
        {
            SceneManager.LoadSceneAsync(sceneInfo.SceneName, loadMode);
            //TODO: add loading panel and fade
        }

        public void UnloadSceneAsync(SceneInfo sceneInfo)
        {
            SceneManager.UnloadSceneAsync(sceneInfo.SceneName);
        }
    }
}