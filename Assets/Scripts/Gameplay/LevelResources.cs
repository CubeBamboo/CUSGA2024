using CbUtils;
using Shuile.Gameplay;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using UnityEngine.AddressableAssets;

namespace Shuile
{
    public class LevelResources : MonoSingletons<LevelResources>
    {
        public AssetReference chartAssets;
        //public ChartSO chartSO;

        public ChartData CurrentChart;

        protected override void OnAwake()
        {
            CurrentChart = ChartUtils.LoadChart(chartAssets);
            GameplayService.Interface.OnInit();
        }
        private void OnDestroy()
        {
            GameplayService.Interface.OnDeInit();
        }
    }
}
