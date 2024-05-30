using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Rhythm.Runtime;

namespace Shuile
{
    public class MonoEnemySpawnTrigger : MonoEntity
    {
        private EnemySpawnManager _enemySpawnManager;
        private AutoPlayChartManager _autoPlayChartManager;

        protected override void AwakeOverride()
        {
            _enemySpawnManager = this.GetSystem<EnemySpawnManager>();
            _autoPlayChartManager = this.GetSystem<AutoPlayChartManager>();
        }
        private void Start()
        {
            _autoPlayChartManager.OnRhythmHit += _enemySpawnManager.OnRhythmHit;
        }
        protected override void OnDestroyOverride()
        {
            _autoPlayChartManager.OnRhythmHit += _enemySpawnManager.OnRhythmHit;
        }

        public override LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;
    }
}
