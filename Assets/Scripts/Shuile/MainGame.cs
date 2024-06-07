using Cysharp.Threading.Tasks;
using Shuile.Framework;
using Shuile.Persistent;

namespace Shuile
{
    public class MainGame : AbstractLocator<MainGame>
    {
        public override bool InitOnApplicationAwake => true;

        public override void OnInit()
        {
            var localConfigAccessor = new PlayerPrefsAccessor<Config>("Config");
            this.Register<IAccessor<Config>>(localConfigAccessor);
            this.Register<PlayerPrefsAccessor<Config>>(localConfigAccessor);

            AsyncServiceLoader().Forget();
        }

        public override void OnDeInit()
        {
            this.UnRegister<IAccessor<Config>>();
            this.UnRegister<PlayerPrefsAccessor<Config>>();
            this.UnRegister<Config>();
        }

        private async UniTask AsyncServiceLoader()
        {
            var configLoaderTask = this.Get<IAccessor<Config>>().LoadAsync().ContinueWith(config => this.Register(config));

            await UniTask.WhenAll(configLoaderTask);
        }
    }
}
