using System.Threading;

namespace Shuile.Core.Global
{
    public class SceneTransitionManager
    {
        private CancellationTokenSource _isSceneChanged = new();

        public CancellationToken SceneChangedToken => _isSceneChanged.Token;

        public void OnNotifySceneChanged()
        {
            _isSceneChanged.Cancel();
            _isSceneChanged.Dispose();
            _isSceneChanged = new CancellationTokenSource();
        }
    }
}
