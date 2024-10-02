using Shuile.Audio;
using Shuile.Core.Framework.Unity;
using Shuile.Framework;
using Shuile.Gameplay.Manager;
using Shuile.Rhythm;
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
        [SerializeField] private MusicRhythmManager musicRhythmManager;
        [SerializeField] private LevelGlobalManager levelGlobalManager;

        public override void Configure(IRegisterableScope scope)
        {
            scope.RegisterMonoComponent(levelAudioManager);
            scope.RegisterMonoComponent(levelGlobalManager);

            scope.RegisterEntryPoint(() => new PreciseMusicPlayer(ContainerExtensions.FindSceneContext()));
        }
    }
}
