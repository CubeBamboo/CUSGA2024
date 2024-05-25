using CbUtils;
using CbUtils.Extension;
using Shuile.Gameplay;
using Shuile.ResourcesManagement.Loader;
using Shuile.Rhythm.Runtime;
using Shuile.Utils;
using UnityEngine;

namespace Shuile
{
    public class LevelSingletonsSpawner : MonoBehaviour
    {
        private Transform dependencyParent; // not singleton
        private Transform singletonParent; // singleton

        public void Awake()
        {
            dependencyParent = new GameObject($"{nameof(dependencyParent)}").SetParent(transform).transform;
            singletonParent = new GameObject($"{nameof(singletonParent)}").SetParent(transform).transform;

            LevelResourcesLoader.Instance.SyncContext.levelAudioManager.Instantiate().SetParent(singletonParent);

            new GameObject($"{nameof(PreciseMusicPlayer)}", typeof(PreciseMusicPlayer)).SetParent(dependencyParent);

            new GameObject($"{nameof(AutoPlayChartManager)}", typeof(AutoPlayChartManager)).SetParent(singletonParent);
            new GameObject($"{nameof(PlayerChartManager)}", typeof(PlayerChartManager)).SetParent(singletonParent);
            new GameObject($"{nameof(LevelChartManager)}", typeof(LevelChartManager)).SetParent(singletonParent);
            new GameObject($"{nameof(EnemySpawnManager)}", typeof(EnemySpawnManager)).SetParent(singletonParent);
            new GameObject($"{nameof(EntityManager)}", typeof(EntityManager)).SetParent(singletonParent);
            new GameObject($"{nameof(LevelFeelManager)}", typeof(LevelFeelManager)).SetParent(singletonParent);
            new GameObject($"{nameof(LevelGlobalManager)}", typeof(LevelGlobalManager)).SetParent(singletonParent);

            LevelStateMachine.Instance.enabled = true;
        }
        public void OnDestroy()
        {
            Destroy(dependencyParent.gameObject);
            Destroy(singletonParent.gameObject);
        }
    }
}
