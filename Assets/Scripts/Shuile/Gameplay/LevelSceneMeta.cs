using Shuile.Gameplay.Model;
using Shuile.UI;

namespace Shuile.Gameplay
{
    public class LevelSceneMeta : MonoGameRouter.SceneMeta
    {
        public LevelSceneMeta(SingleLevelData data) : base(data.LevelData.sceneName)
        {
            SceneContext.RegisterInstance(data);
        }
    }

    public class EndStaticsSceneMeta : MonoGameRouter.SceneMeta
    {
        public EndStaticsSceneMeta(EndStaticsPanel.Data data) : base("EndStatics")
        {
            SceneContext.RegisterInstance(data);
        }
    }
}
