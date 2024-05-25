using Shuile.Framework;

namespace Shuile.Gameplay
{
    [System.Obsolete("no sublayer, not good, use GameApplication.ServiceLocator instead")] // if you dont want to use sublayer, choose singleton
    public class GameplayService : AbstractLocator<GameplayService>
    {
        public override bool InitOnApplicationAwake => false;

        public override void OnInit()
        {
            this.Register<IRouteFinder>(new SimpleRouteFinder());
        }

        public override void OnDeInit()
        {
            this.UnRegister<IRouteFinder>();
        }
    }
}
