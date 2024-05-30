using CbUtils.Unity;

namespace Shuile
{
    public class GameRoot : MonoSingletons<GameRoot>
    {
        private void Start()
        {
            MonoGameRouter.Instance.LoadMenu();
        }
    }
}
