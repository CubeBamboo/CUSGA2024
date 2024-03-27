using CbUtils;
using Shuile.Audio;
using UnityEngine;

namespace Shuile.Framework
{
    public class MainGame : AbstractLocator<MainGame>
    {
        protected override void OnInit()
        {
            // global application
            this.Register<ISceneLoader>(new SceneLoaderManager());
            this.Register<IAudioPlayer>(new SimpleAudioPlayer());
        }
    }
}
