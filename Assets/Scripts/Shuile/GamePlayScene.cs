using Shuile.Audio;
using Shuile.Core.Gameplay;
using Shuile.Framework;
using Shuile.Gameplay.Character;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Feel;
using Shuile.Gameplay.Manager;
using Shuile.Gameplay.Model;
using Shuile.Model;
using Shuile.Rhythm;
using Shuile.UI.Gameplay;
using Shuile.Utils;
using UnityEngine;

namespace Shuile
{
    public class GamePlayScene : SceneContainer
    {
        [SerializeField] private Player player;
        [SerializeField] private EndLevelPanel endLevelPanel;
        [SerializeField] private LevelAudioManager levelAudioManager;
        [SerializeField] private Transform enemyParent;

        [SerializeField] private GameObject preLoadGameObject;

        public override void BuildSelfContext(RuntimeContext context)
        {
            context.RegisterFactory(() => new LevelFeelManager());
            context.RegisterFactory(() => new LevelStateMachine());
            context.RegisterFactory(() => new LevelTimingManager(context));

            context.RegisterMonoScheduler(this);

            context.Resolve(out SingleLevelData runtimeLevelData);
            var statics = new GameplayStatics { SongName = runtimeLevelData.SongName, Composer = runtimeLevelData.Composer };

            context.RegisterInstance(statics);

            context.RegisterInstance(this);
            context.RegisterInstance(preLoadGameObject.GetComponent<MusicRhythmManager>().ThrowIfNull());
            var zoneManager = preLoadGameObject.GetComponent<LevelZoneManager>().ThrowIfNull();
            context.RegisterInstance(zoneManager);
            context.RegisterInstance((ILevelZoneManager)zoneManager);
            context.RegisterInstance(preLoadGameObject.GetComponent<MonoAudioChannel>().ThrowIfNull());
            context.RegisterInstance(endLevelPanel);
            context.RegisterInstance(levelAudioManager);
            context.RegisterInstance(new LevelModel());
            context.RegisterInstance(new LevelEntityManager(context));
            context.RegisterInstance(new PreciseMusicPlayer(context));

            context.RegisterInstance(new LevelModel.TimingData(runtimeLevelData.ChartData.time));

            context.RegisterInstance(new AutoPlayChartManager(context));
            context.RegisterInstance(new LevelChartManager(context));
            context.RegisterInstance(new EnemySpawnManager(context, enemyParent));
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

        public class GameplayStatics
        {
            public string SongName;
            public string Composer;

            public int TotalHit;
            public int HitOnRhythm;
            public int TotalKillEnemy;
            public int Score;
            public int HealthLoss;
        }
    }
}
