using Shuile.Core.Gameplay;
using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.Gameplay.Character;
using Shuile.Gameplay.Feel;
using Shuile.Gameplay.Manager;
using Shuile.Rhythm;
using UnityEngine;

namespace Shuile
{
    public class GamePlayScene : SceneContainer
    {
        [SerializeField] private Player player;
        [SerializeField] private MusicRhythmManager musicRhythmManager;

        public override void BuildContext(ServiceLocator context)
        {
            context.RegisterMonoScheduler(this);

            context.RegisterInstance(this);
            context.RegisterInstance(musicRhythmManager);

            var scope = LevelScope.Interface;
            context.RegisterInstance(new AutoPlayChartManager(scope, context));
            context.RegisterInstance(new LevelChartManager(scope, context));
            context.RegisterInstance(new EnemySpawnManager(scope, context));

            context.RegisterFactory(() => new LevelFeelManager());
            context.RegisterFactory(() => new LevelStateMachine());
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
