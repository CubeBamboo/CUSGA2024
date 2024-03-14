using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shuile.Framework
{
    public interface ISceneLoader
    {
        public void LoadSceneAsync(SceneInfo sceneInfo, LoadSceneMode loadMode = LoadSceneMode.Single, bool showLoadingPanel = false, bool fadeInOut = true);
        public void UnloadSceneAsync(SceneInfo sceneInfo);
    }

    public struct SceneInfo
    {
        public string SceneName;
        // TODO: add more scene info, like asset reference.
    }
}
