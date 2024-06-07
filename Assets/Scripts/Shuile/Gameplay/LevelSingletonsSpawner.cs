using CbUtils.Extension;
using Shuile.Gameplay.Manager;
using Shuile.Rhythm.Runtime;
using UnityEngine;

namespace Shuile.Gameplay
{
    public class LevelSingletonsSpawner : MonoBehaviour
    {
        private Transform _dependencyParent; // not singleton
        private Transform _singletonParent; // singleton

        public void Awake()
        {
            _dependencyParent = new GameObject($"{nameof(_dependencyParent)}").SetParent(transform).transform;
            _singletonParent = new GameObject($"{nameof(_singletonParent)}").SetParent(transform).transform;

            new GameObject($"{nameof(AutoPlayChartManagerUpdater)}", typeof(AutoPlayChartManagerUpdater))
                .SetParent(_singletonParent);
            new GameObject($"{nameof(PlayerChartManagerUpdater)}", typeof(PlayerChartManagerUpdater))
                .SetParent(_singletonParent);
            new GameObject($"{nameof(MonoEnemySpawnTrigger)}", typeof(MonoEnemySpawnTrigger))
                .SetParent(_singletonParent);
            new GameObject($"{nameof(LevelChartManager)}", typeof(LevelChartManager))
                .SetParent(_singletonParent);
            new GameObject($"{nameof(LevelEntityManager)}", typeof(LevelEntityManager))
                .SetParent(_singletonParent);
            new GameObject($"{nameof(LevelGlobalManager)}", typeof(LevelGlobalManager))
                .SetParent(_singletonParent);
        }

        public void OnDestroy()
        {
            Destroy(_dependencyParent.gameObject);
            Destroy(_singletonParent.gameObject);
        }
    }
}
