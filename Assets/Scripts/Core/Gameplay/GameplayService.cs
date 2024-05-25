using Shuile.Framework;
using Shuile.Rhythm;

namespace Shuile.Gameplay
{
    public class GameplayService : AbstractLocator<GameplayService>
    {
        public override bool InitOnApplicationAwake => false;

        public override void OnInit()
        {
            this.Register<PlayerModel>(new PlayerModel());
            this.Register<LevelModel>(new LevelModel());
            this.Register<IRouteFinder>(new SimpleRouteFinder());
            this.Register<LevelTimingManager>(new LevelTimingManager());
        }

        public override void OnDeInit()
        {
            this.UnRegister<PlayerModel>();
            this.UnRegister<LevelModel>();
            this.UnRegister<IRouteFinder>();
            this.UnRegister<LevelTimingManager>();
        }
    }
}
