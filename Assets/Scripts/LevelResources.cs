using CbUtils;
using Shuile.Gameplay;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UDebug = UnityEngine.Debug;

namespace Shuile
{
    // use unity monobehavior to manage the resources
    public class LevelResources : MonoSingletons<LevelResources>
    {
        public MusicManagerConfigSO musicManagerConfig;
        public LevelConfigSO levelConfig;
        public PlayerSettingsConfigSO playerConfig;
        [Tooltip("for debug")] public LevelDebugSettingsSO debugSettings;

        public PrefabConfigSO globalPrefabs;

        protected override void OnAwake()
        {
            GameplayService.Interface.OnInit();
        }
        private void OnDestroy()
        {
            GameplayService.Interface.OnDeInit();
        }

        protected override void OnInstanceCall(bool isNewObject)
        {
            if (isNewObject)
            {
                UDebug.LogWarning("[For tester] LevelResources is not init by scene, load \"Root\" scene first");
            }
        }
    }
}
