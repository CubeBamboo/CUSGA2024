using CbUtils.Unity;
using UnityEngine.SceneManagement;

namespace Shuile
{
    public class GameRoot : MonoSingletons<GameRoot>
    {
        private void Start()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
