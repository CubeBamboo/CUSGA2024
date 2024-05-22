using CbUtils.Extension;
using Shuile.Gameplay;
using Shuile.ResourcesManagement.Loader;
using Shuile.Rhythm.Runtime;
using UnityEngine;

namespace Shuile
{
    public class LevelSingletonsSpawner : MonoBehaviour
    {
        private void Awake()
        {
            LevelResourcesLoader.Instance.SyncContext.levelAudioManager.Instantiate().SetParent(transform);
            new GameObject($"{nameof(MusicRhythmManager)}", typeof(MusicRhythmManager)).SetParent(transform);
            new GameObject($"{nameof(AutoPlayChartManager)}", typeof(AutoPlayChartManager)).SetParent(transform);
            new GameObject($"{nameof(PlayerChartManager)}", typeof(PlayerChartManager)).SetParent(transform);
            new GameObject($"{nameof(LevelChartManager)}", typeof(LevelChartManager)).SetParent(transform);
            new GameObject($"{nameof(EnemySpawnManager)}", typeof(EnemySpawnManager)).SetParent(transform);
            new GameObject($"{nameof(EntityManager)}", typeof(EntityManager)).SetParent(transform);
            new GameObject($"{nameof(LevelFeelManager)}", typeof(LevelFeelManager)).SetParent(transform);
            new GameObject($"{nameof(LevelGlobalManager)}", typeof(LevelGlobalManager)).SetParent(transform);
        }
    }
}
