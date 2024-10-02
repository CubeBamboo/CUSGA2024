using Shuile.Audio;
using Shuile.Core.Gameplay;
using Shuile.Framework;
using Shuile.Gameplay.Character;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Feel;
using Shuile.Gameplay.Manager;
using Shuile.Model;
using Shuile.Rhythm;
using Shuile.UI.Gameplay;
using UnityEngine;

namespace Shuile
{
    public class GamePlayScene : SceneContainer
    {
        [SerializeField] private Player player;
        [SerializeField] private MusicRhythmManager musicRhythmManager;
        [SerializeField] private LevelZoneManager levelZoneManager;
        [SerializeField] private EndLevelPanel endLevelPanel;
        [SerializeField] private LevelAudioManager levelAudioManager;
        [SerializeField] private Transform enemyParent;

        public override void BuildSelfContext(RuntimeContext context)
        {
            context.RegisterFactory(() => new LevelFeelManager());
            context.RegisterFactory(() => new LevelStateMachine());
            context.RegisterFactory(() => new LevelTimingManager(context));

            context.RegisterMonoScheduler(this);

            context.RegisterInstance(this);
            context.RegisterInstance(musicRhythmManager);
            context.RegisterInstance(levelZoneManager);
            context.RegisterInstance(endLevelPanel);
            context.RegisterInstance(levelAudioManager);
            context.RegisterInstance(new LevelModel());
            context.RegisterInstance(new LevelEntityManager(context, enemyParent));
            context.RegisterInstance(new PreciseMusicPlayer(context));

            context.RegisterInstance(new AutoPlayChartManager(context));
            context.RegisterInstance(new LevelChartManager(context));
            context.RegisterInstance(new EnemySpawnManager(context));
        }

        // player can be null
        public bool TryGetPlayer(out Player instance)
        {
            if (player != null)
            {
                instance = player;
                return true;
            }

            instance = player = GetComponentInChildren<Player>();
            return player == null;
        }
    }
}
