using CbUtils;
using Shuile.Core.Gameplay;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Shuile.ResourcesManagement.Loader
{
    public class GameResourcesLoader : CSharpLazySingletons<GameResourcesLoader>
    {
        private LevelDataMapSO levelDataMap;

        private static readonly System.Func<Task<LevelDataMapSO>> levelDataMapFactory =
            () => Addressables.LoadAssetAsync<LevelDataMapSO>("Data/LevelDataMap.asset").Task;

        public async Task PreCacheAsync()
        {
            levelDataMap = levelDataMap ? levelDataMap : await levelDataMapFactory();
        }

        public async Task<LevelDataMapSO> GetLevelDataMapAsync() => levelDataMap = levelDataMap ? levelDataMap : await levelDataMapFactory();
    }
}
