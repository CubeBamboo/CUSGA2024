using Shuile.Audio;

namespace Shuile.Framework
{
    public class MainGame : AbstractLocator<MainGame>
    {
        public override bool IsGlobal => true;

        public override void OnInit()
        {
            this.Register<IAudioPlayer>(new SimpleAudioPlayer());
        }

        public override void OnDeInit()
        {
            this.UnRegister<IAudioPlayer>();
        }
    }
}
