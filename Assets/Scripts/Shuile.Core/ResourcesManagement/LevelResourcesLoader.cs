using CbUtils;
using Shuile.Core.Global;
using Shuile.Core.Global.Config;
using Shuile.Utils;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Shuile.ResourcesManagement.Loader
{
    public struct LevelResourcesContent
    {
        public LevelConfigSO levelConfig;
        public static readonly System.Func<Task<LevelConfigSO>> levelConfigFactory =
            () => Addressables.LoadAssetAsync<LevelConfigSO>("Data/MainLevelConfig.asset").Task;

        public PrefabConfigSO globalPrefabs;
        public static readonly System.Func<Task<PrefabConfigSO>> globalPrefabsFactory =
            () => Resources.LoadAsync<PrefabConfigSO>("GlobalPrefabConfig").AsTask<PrefabConfigSO>();
    }

    public class LevelResourcesLoader : CSharpLazySingletons<LevelResourcesLoader>
    {
        private LevelResourcesContent syncContext;
        public LevelResourcesContent SyncContext => syncContext;

        public async Task PreCacheAsync()
        {
            syncContext = new LevelResourcesContent
            {
                levelConfig = syncContext.levelConfig ? syncContext.levelConfig : await LevelResourcesContent.levelConfigFactory(),
                globalPrefabs = syncContext.globalPrefabs ? syncContext.globalPrefabs : await LevelResourcesContent.globalPrefabsFactory(),
            };
        }

        public async Task<LevelConfigSO> GetLevelConfigAsync() => syncContext.levelConfig = syncContext.levelConfig ? syncContext.levelConfig : await LevelResourcesContent.levelConfigFactory();
        public async Task<PrefabConfigSO> GetGlobalPrefabsAsync() => syncContext.globalPrefabs = syncContext.globalPrefabs ? syncContext.globalPrefabs : await LevelResourcesContent.globalPrefabsFactory();
    }
}
