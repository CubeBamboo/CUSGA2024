using CbUtils;
using Shuile.Core.Gameplay.Data;
using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Shuile.ResourcesManagement.Loader
{
    public class GameResourcesLoader : CSharpLazySingletons<GameResourcesLoader>
    {
        private static readonly Func<Task<LevelDataMapSO>> levelDataMapFactory =
            () => Addressables.LoadAssetAsync<LevelDataMapSO>("Data/LevelDataMap.asset").Task;

        private LevelDataMapSO levelDataMap;

        public async Task PreCacheAsync()
        {
            levelDataMap = levelDataMap ? levelDataMap : await levelDataMapFactory();
        }

        public async Task<LevelDataMapSO> GetLevelDataMapAsync()
        {
            return levelDataMap = levelDataMap ? levelDataMap : await levelDataMapFactory();
        }
    }
}
