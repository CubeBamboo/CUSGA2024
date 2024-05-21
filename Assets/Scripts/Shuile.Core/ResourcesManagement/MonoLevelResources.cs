using CbUtils;
using UDebug = UnityEngine.Debug;

namespace Shuile
{
    // use unity monobehavior to manage the resources
    public class MonoLevelResources : MonoSingletons<MonoLevelResources>
    {
        public LevelConfigSO levelConfig;

        public PrefabConfigSO globalPrefabs;

        protected override void OnInstanceCall(bool isNewObject)
        {
            if (isNewObject)
            {
                UDebug.LogWarning("[For tester] LevelResources is not init by scene, load \"Root\" scene first");
            }
        }
    }
}
