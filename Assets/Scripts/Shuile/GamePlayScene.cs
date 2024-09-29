using Shuile.Core.Framework;
using Shuile.Framework;
using Shuile.Gameplay.Feel;

namespace Shuile
{
    public class GamePlayScene : SceneContainer
    {
        public override void BuildContext(ServiceLocator context)
        {
            context.RegisterInstance(new LevelFeelManager());
        }
    }
}
