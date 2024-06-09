using Shuile.Audio;
using Shuile.Chart;
using Shuile.Core.Framework.Unity;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Manager;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using UnityEngine;

namespace Shuile.Gameplay
{
    public class LevelScope: SceneServiceScope<LevelScope>
    {
        [SerializeField] private LevelAudioManager levelAudioManager;
        
        public override void Configure(LevelScope scope)
        {
            scope.Register<NoteDataProcessor>(() => new NoteDataProcessor(scope.Get<LevelEntityManager>()));
            
            scope.RegisterMonoComponent<LevelAudioManager>(levelAudioManager);
            
            scope.RegisterEntryPoint<PreciseMusicPlayer>(() => new PreciseMusicPlayer());
            scope.RegisterEntryPoint<PlayerChartManager>(() => new PlayerChartManager());
            scope.RegisterEntryPoint<AutoPlayChartManager>(() => new AutoPlayChartManager());
            scope.RegisterEntryPoint<EnemySpawnManager>(() => new EnemySpawnManager());
            scope.RegisterEntryPoint<LevelChartManager>(() => new LevelChartManager());
            scope.RegisterEntryPoint<LevelEntityManager>(() => new LevelEntityManager());
        }
    }
}
