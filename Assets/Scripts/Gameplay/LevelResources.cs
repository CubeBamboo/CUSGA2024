using CbUtils;
using Shuile.Gameplay;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Shuile
{
    public class LevelResources : MonoSingletons<LevelResources>
    {
        public AssetReference chartAssets;
        public MusicManagerConfigSO musicManagerConfig;
        public LevelConfigSO levelConfig;
        public PlayerSettingsConfigSO playerConfig;
        [Tooltip("for debug")] public LevelDebugSettingsSO debugSettings;

        public ChartData currentChart;

        protected override void OnAwake()
        {
            currentChart = ChartUtils.LoadChart(chartAssets);
            GameplayService.Interface.OnInit();
        }
        private void OnDestroy()
        {
            GameplayService.Interface.OnDeInit();
        }
    }
}
