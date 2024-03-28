using Shuile.Audio;

namespace Shuile.Framework
{
    public class MainGame : AbstractLocator<MainGame>
    {
        public override bool IsGlobal => true;

        public override void OnInit()
        {
            // global application
            this.Register<ISceneLoader>(new SceneLoaderManager());
            this.Register<IAudioPlayer>(new SimpleAudioPlayer());
        }

        public override void OnDeInit()
        {
            this.UnRegister<ISceneLoader>();
            this.UnRegister<IAudioPlayer>();
        }
    }
}
