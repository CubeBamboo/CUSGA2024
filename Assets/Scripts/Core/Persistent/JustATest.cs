using Cysharp.Threading.Tasks;

using Shuile.Framework;

using UnityEngine;

namespace Shuile.Persistent
{
    public class JustATest : MonoBehaviour
    {
        private void Start()
        {
            var configViewer = MainGame.Interface.CreatePersistentDataViewer<Config, MainGame>();
            configViewer.AutoSave = true;
            configViewer.OnDirtyStateChanged += OnDirty;
            Debug.Log($"Load config delay is {configViewer.Data.Audio.GlobalDelay}ms");
            configViewer.Data.Audio.GlobalDelay += 20;

            /*Debug.Log("Saving manual");
            configViewer.SaveAsync().Forget();
*/
            var config = MainGame.Interface.Get<Config>();
            Debug.Log($"config delay ima wa {config.Audio.GlobalDelay}");
        }

        private void OnDirty(bool dirty)
        {
            Debug.Log($"Config dirty state changed, ima wa {dirty}");
        }
    }
}