using Shuile.Audio;

namespace Shuile.Framework
{
    public class MainGame : AbstractLocator<MainGame>
    {
        protected override void OnInit()
        {
            this.Register<ISceneLoader>(new SceneLoaderManager());
            this.Register<IAudioPlayer>(new SimpleAudioPlayer());
        }
    }
}
