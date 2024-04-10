using CbUtils;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Shuile
{
    public class GameRoot : MonoSingletons<GameRoot>
    {
        protected override void OnAwake()
        {
            SetDontDestroyOnLoad();
        }
        private void Start()
        {
            LoadLevel();
        }

        public void LoadLevel()
        {
            var dependcyHandle = Addressables.LoadSceneAsync("LevelDependency.unity", LoadSceneMode.Single).WaitForCompletion();
            Addressables.LoadSceneAsync("Level0Test.unity", LoadSceneMode.Additive).Completed += op =>
            {
                SceneManager.SetActiveScene(op.Result.Scene);
            };
        }
    }
}
