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
        [SerializeField] private LevelZoneManager levelZoneManager;
        
        public override void Configure(IRegisterableScope scope)
        {
            scope.Register<NoteDataProcessor>(() => new NoteDataProcessor(this));
            
            scope.RegisterMonoComponent<LevelAudioManager>(levelAudioManager);
            scope.RegisterMonoComponent<LevelZoneManager>(levelZoneManager);
            
            scope.RegisterEntryPoint<PreciseMusicPlayer>(() => new PreciseMusicPlayer(this));
            scope.RegisterEntryPoint<PlayerChartManager>(() => new PlayerChartManager(this));
            scope.RegisterEntryPoint<LevelEntityManager>(() => new LevelEntityManager(this));
            scope.RegisterEntryPoint<AutoPlayChartManager>(() => new AutoPlayChartManager(this));
            scope.RegisterEntryPoint<EnemySpawnManager>(() => new EnemySpawnManager(this));
            scope.RegisterEntryPoint<LevelChartManager>(() => new LevelChartManager(this));
        }
    }
}
