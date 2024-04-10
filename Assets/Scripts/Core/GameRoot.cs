using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Shuile
{
    public class GameRoot : MonoBehaviour
    {
        private void Start()
        {
            LoadLevel();
        }

        private void LoadLevel()
        {
            Addressables.LoadSceneAsync("LevelDependency.unity", LoadSceneMode.Single).Completed += op =>
            {
                Addressables.LoadSceneAsync("Level0Test.unity", LoadSceneMode.Additive).Completed += op =>
                {
                    SceneManager.SetActiveScene(op.Result.Scene);
                };
            };
        }
    }
}
