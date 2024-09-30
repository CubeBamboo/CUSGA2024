using Shuile.Audio;
using Shuile.Chart;
using Shuile.Core.Framework.Unity;
using Shuile.Gameplay.Character;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Feel;
using Shuile.Gameplay.Manager;
using Shuile.Model;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using Shuile.UI.Gameplay;
using System;
using UnityEngine;

namespace Shuile.Gameplay
{
    // we are now work in progress with the new architecture named MonoContainer, to replace the service scope.
    [Obsolete]
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

            scope.RegisterMonoComponent(levelAudioManager);
            scope.RegisterMonoComponent(levelZoneManager);
            scope.RegisterMonoComponent(endLevelPanel);
            scope.RegisterMonoComponent(player);
            scope.RegisterMonoComponent(levelGlobalManager);

            scope.RegisterEntryPoint(() => new PreciseMusicPlayer(this));
            scope.RegisterEntryPoint(() => new LevelEntityManager(this));
        }
    }
}
