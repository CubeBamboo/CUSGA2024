using Cysharp.Threading.Tasks;
using Shuile.Framework;
using Shuile.Persistent;

namespace Shuile
{
    public class MainGame : AbstractLocator<MainGame> // nearly obsolete, use GameApplication.Global instead
    {
        public override bool InitOnApplicationAwake => true;

        public override void OnInit()
        {
            var localConfigAccessor = new PlayerPrefsAccessor<Config>("Config");
            Register<IAccessor<Config>>(localConfigAccessor);
            Register(localConfigAccessor);

            AsyncServiceLoader().Forget();
        }

        public override void OnDeInit()
        {
            UnRegister<IAccessor<Config>>();
            UnRegister<PlayerPrefsAccessor<Config>>();
            UnRegister<Config>();
        }

        private async UniTask AsyncServiceLoader()
        {
            var configLoaderTask = Get<IAccessor<Config>>().LoadAsync().ContinueWith(config => Register(config));

            await UniTask.WhenAll(configLoaderTask);
        }
    }
}
