using Shuile.Gameplay.Model;

namespace Shuile.Gameplay
{
    public class LevelSceneMeta : MonoGameRouter.SceneMeta
    {
        public LevelSceneMeta(LevelContext context) : base(context.LevelData.sceneName)
        {
            Context.RegisterInstance(context);
        }
    }
}
