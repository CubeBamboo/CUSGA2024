using Cysharp.Threading.Tasks;

using Shuile.Audio;
using Shuile.Persistent;

namespace Shuile.Framework
{
    public class MainGame : AbstractLocator<MainGame>
    {
        public override bool IsGlobal => true;

        public override void OnInit()
        {
            // global application
            //this.Register<ISceneLoader>(new SceneLoaderManager());
            this.Register<IAudioPlayer>(new SimpleAudioPlayer());

            var localConfigAccessor = new PlayerPrefsAccessor<Config>("Config");
            this.Register<IAccessor<Config>>(localConfigAccessor);
            this.Register<PlayerPrefsAccessor<Config>>(localConfigAccessor);

            AsyncServiceLoader().Forget();
        }

        public override void OnDeInit()
        {
            //this.UnRegister<ISceneLoader>();
            this.UnRegister<IAudioPlayer>();
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
