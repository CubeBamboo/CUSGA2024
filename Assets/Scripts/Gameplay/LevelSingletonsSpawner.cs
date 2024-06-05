using CbUtils.Extension;
using Shuile.Gameplay;
using Shuile.ResourcesManagement.Loader;
using Shuile.Rhythm.Runtime;
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

            new GameObject($"{nameof(PreciseMusicPlayer)}", typeof(PreciseMusicPlayer)).SetParent(dependencyParent);

            new GameObject($"{nameof(AutoPlayChartManagerUpdater)}", typeof(AutoPlayChartManagerUpdater)).SetParent(singletonParent);
            new GameObject($"{nameof(PlayerChartManagerUpdater)}", typeof(PlayerChartManagerUpdater)).SetParent(singletonParent);
            new GameObject($"{nameof(MonoEnemySpawnTrigger)}", typeof(MonoEnemySpawnTrigger)).SetParent(singletonParent);
            new GameObject($"{nameof(LevelChartManager)}", typeof(LevelChartManager)).SetParent(singletonParent);
            new GameObject($"{nameof(LevelEntityManager)}", typeof(LevelEntityManager)).SetParent(singletonParent);
            new GameObject($"{nameof(LevelGlobalManager)}", typeof(LevelGlobalManager)).SetParent(singletonParent);
        }
        public void OnDestroy()
        {
            Destroy(dependencyParent.gameObject);
            Destroy(singletonParent.gameObject);
        }
    }
}
