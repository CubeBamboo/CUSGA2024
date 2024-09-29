using Shuile.Framework;
using Shuile.Gameplay.Feel;

namespace Shuile
{
    public class GamePlayScene : MonoContainer
    {
        public override void BuildContext(ServiceLocator context)
        {
            context.RegisterInstance(new LevelFeelManager());
        }
    }
}
