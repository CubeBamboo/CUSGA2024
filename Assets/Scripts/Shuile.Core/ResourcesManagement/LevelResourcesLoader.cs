using CbUtils;
using Shuile.Utils;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Shuile.ResourcesManagement.Loader
{
    public class LevelResourcesLoader : CSharpLazySingletons<LevelResourcesLoader>
    {
        private LevelConfigSO levelConfig;
        private static readonly System.Func<Task<LevelConfigSO>> levelConfigFactory =
            () => Addressables.LoadAssetAsync<LevelConfigSO>("Data/MainLevelConfig.asset").Task;

        private PrefabConfigSO globalPrefabs;
        private static readonly System.Func<Task<PrefabConfigSO>> globalPrefabsFactory =
            () => UnityEngine.Resources.LoadAsync<PrefabConfigSO>("GlobalPrefabConfig").AsTask<PrefabConfigSO>();
            //() => Addressables.LoadAssetAsync<PrefabConfigSO>("Data/GlobalPrefabConfig.asset").Task;

        public async Task PreCacheAsync()
        {
            levelConfig = levelConfig ? levelConfig : await levelConfigFactory();
            globalPrefabs = globalPrefabs ? globalPrefabs : await globalPrefabsFactory();
        }

        public async Task<LevelConfigSO> GetLevelConfigAsync()
        {
            if (!levelConfig) await levelConfigFactory();
            return levelConfig;
        }
        public async Task<PrefabConfigSO> GetGlobalPrefabsAsync()
        {
            if (!globalPrefabs) await globalPrefabsFactory();
            return globalPrefabs;
        }
    }
}
