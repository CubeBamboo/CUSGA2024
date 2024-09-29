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
    public class LevelScope : SceneServiceScope<LevelScope>
    {
        [SerializeField] private LevelAudioManager levelAudioManager;
        [SerializeField] private LevelZoneManager levelZoneManager;
        [SerializeField] private EndLevelPanel endLevelPanel;
        [SerializeField] private Player player;
        [SerializeField] private MusicRhythmManager musicRhythmManager;
        [SerializeField] private LevelGlobalManager levelGlobalManager;

        public override void Configure(IRegisterableScope scope)
        {
            scope.Register(() => new LevelModel());

            scope.Register(() => new LevelFeelManager());
            scope.Register(() => new LevelTimingManager(this));
            scope.Register(() => new NoteDataProcessor(this));

            scope.RegisterMonoComponent(levelAudioManager);
            scope.RegisterMonoComponent(levelZoneManager);
            scope.RegisterMonoComponent(endLevelPanel);
            scope.RegisterMonoComponent(player);
            scope.RegisterMonoComponent(musicRhythmManager);
            scope.RegisterMonoComponent(levelGlobalManager);

            scope.RegisterEntryPoint(() => new PreciseMusicPlayer(this));
            scope.RegisterEntryPoint(() => new PlayerChartManager(this));
            scope.RegisterEntryPoint(() => new LevelEntityManager(this));
            scope.RegisterEntryPoint(() => new AutoPlayChartManager(this));
            scope.RegisterEntryPoint(() => new EnemySpawnManager(this));
            scope.RegisterEntryPoint(() => new LevelChartManager(this));
        }
    }
}
