using Shuile.Audio;
using Shuile.Chart;
using Shuile.Core.Framework.Unity;
using Shuile.Core.Gameplay;
using Shuile.Gameplay.Character;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Feel;
using Shuile.Gameplay.Manager;
using Shuile.Model;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using Shuile.UI.Gameplay;
using UnityEngine;

namespace Shuile.Gameplay
{
    public class LevelScope: SceneServiceScope<LevelScope>
    {
        [SerializeField] private LevelAudioManager levelAudioManager;
        [SerializeField] private LevelZoneManager levelZoneManager;
        [SerializeField] private EndLevelPanel endLevelPanel;
        [SerializeField] private Player player;
        [SerializeField] private MusicRhythmManager musicRhythmManager;
        
        public override void Configure(IRegisterableScope scope)
        {
            scope.Register<PlayerModel>(() => new PlayerModel());
            scope.Register<LevelModel>(() => new LevelModel());
            
            scope.Register<LevelFeelManager>(() => new LevelFeelManager());
            scope.Register<LevelTimingManager>(() => new LevelTimingManager(this));
            scope.Register<NoteDataProcessor>(() => new NoteDataProcessor(this));
            scope.Register<LevelStateMachine>(() => new LevelStateMachine(this));
            
            scope.RegisterMonoComponent<LevelAudioManager>(levelAudioManager);
            scope.RegisterMonoComponent<LevelZoneManager>(levelZoneManager);
            scope.RegisterMonoComponent<EndLevelPanel>(endLevelPanel);
            scope.RegisterMonoComponent<Player>(player);
            scope.RegisterMonoComponent<MusicRhythmManager>(musicRhythmManager);
            
            scope.RegisterEntryPoint<PreciseMusicPlayer>(() => new PreciseMusicPlayer(this));
            scope.RegisterEntryPoint<PlayerChartManager>(() => new PlayerChartManager(this));
            scope.RegisterEntryPoint<LevelEntityManager>(() => new LevelEntityManager(this));
            scope.RegisterEntryPoint<AutoPlayChartManager>(() => new AutoPlayChartManager(this));
            scope.RegisterEntryPoint<EnemySpawnManager>(() => new EnemySpawnManager(this));
            scope.RegisterEntryPoint<LevelChartManager>(() => new LevelChartManager(this));
        }
    }
}
