using System.Threading;
using UnityEngine.SceneManagement;

namespace Shuile.Core.Global
{
    public class SceneTransitionManager
    {
        private CancellationTokenSource _isSceneChanged = new();

        public CancellationToken SceneChangedToken => _isSceneChanged.Token;

        public SceneTransitionManager()
        {
            SceneManager.sceneLoaded += (scene, mode) => OnNotifySceneChanged();
        }

        public void OnNotifySceneChanged()
        {
            _isSceneChanged.Cancel();
            _isSceneChanged.Dispose();
            _isSceneChanged = new CancellationTokenSource();
        }
    }
}
