using Shuile.Chart;
using Shuile.Core.Framework;
using Shuile.Core.Global.Config;
using Shuile.Gameplay;
using Shuile.ResourcesManagement.Loader;
using UnityEngine;

namespace Shuile.Rhythm.Runtime
{
    class PlayerChartManagerUpdater : MonoBehaviour, IEntity
    {
        private PlayerChartManager _playerChartManager;
        private MusicRhythmManager _musicRhythmManager;

        private void Awake()
        {
            _playerChartManager = this.GetSystem<PlayerChartManager>();
            _musicRhythmManager = MusicRhythmManager.Instance;
        }
        private void FixedUpdate()
        {
            if (!LevelRoot.Instance.IsStart) return;

            _playerChartManager.ChartPlayer.PlayUpdate(_musicRhythmManager.CurrentTime);
            _playerChartManager.noteContainer.CheckRelease(_musicRhythmManager.CurrentTime);
        }
        public ModuleContainer GetModule() => GameApplication.Level;
    }

    // manage chart of player, convert chart to runtime note object noteContainer
    public class PlayerChartManager : ISystem
    {
        private System.Lazy<ChartPlayer> chartPlayer;

        // chart part
        private readonly ChartData chart = ChartDataCreator.CreatePlayerDefault();

        private float notePreShowInterval = 0.4f;
        private LevelConfigSO _levelConfig;

        public event System.Action OnPlayerHitOn;
        public NoteContainer noteContainer { get; private set;}

        public PlayerChartManager()
        {
            var levelTimingManager = this.GetSystem<LevelTimingManager>();

            _levelConfig = LevelResourcesLoader.Instance.SyncContext.levelConfig;
            notePreShowInterval = _levelConfig.playerNotePreShowTime;

            noteContainer = new();
            chartPlayer = new(() => new ChartPlayer(chart,
                note => note.GetRealTime(levelTimingManager) - notePreShowInterval));
            ChartPlayer.OnNotePlay += (note, _) => noteContainer.AddNote(note.GetRealTime(levelTimingManager));
        }

        public NoteContainer NoteContainer => noteContainer;
        public ChartPlayer ChartPlayer => chartPlayer.Value;
        public int Count => noteContainer.Count;
        public SingleNote TryGetNearestNote(float currentTime) => noteContainer.TryGetNearestNote(currentTime);
        public void HitNote(SingleNote note)
        {
            OnPlayerHitOn?.Invoke();
            noteContainer.ReleaseNote(note);
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
