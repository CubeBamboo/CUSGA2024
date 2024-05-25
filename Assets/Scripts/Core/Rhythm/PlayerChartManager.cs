using DG.Tweening.Plugins.Options;
using Shuile.Core;
using Shuile.Core.Framework;
using Shuile.Gameplay;
using Shuile.Model;
using Shuile.ResourcesManagement.Loader;
using Shuile.Root;

namespace Shuile.Rhythm.Runtime
{
    class PlayerChartManagerUpdater : MonoEntity
    {
        private PlayerChartManager _playerChartManager;
        protected override void AwakeOverride()
        {
            _playerChartManager = this.GetSystem<PlayerChartManager>();
        }
        private void FixedUpdate()
        {
            _playerChartManager.OnFixedUpdate();
        }
        public override LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;
    }

    // manage chart of player, convert chart to runtime note object noteContainer
    public class PlayerChartManager : ISystem
    {
        private MusicRhythmManager _musicRhythmManager;

        private System.Lazy<ChartPlayer> chartPlayer;

        // chart part
        private readonly ChartData chart = ChartDataCreator.CreatePlayerDefault();

        private float notePreShowInterval = 0.4f;
        private LevelConfigSO _levelConfig;

        private NoteContainer noteContainer;
        public event System.Action OnPlayerHitOn;

        public PlayerChartManager()
        {
            _musicRhythmManager = this.GetSystem<MusicRhythmManager>();
            var levelTimingManager = this.GetSystem<LevelTimingManager>();

            _levelConfig = LevelResourcesLoader.Instance.SyncContext.levelConfig;
            notePreShowInterval = _levelConfig.playerNotePreShowTime;

            noteContainer = new();
            chartPlayer = new(() => new ChartPlayer(chart,
                note => note.GetRealTime(levelTimingManager) - notePreShowInterval));
            ChartPlayer.OnNotePlay += (note, _) => noteContainer.AddNote(note.GetRealTime(levelTimingManager));
        }
        public void OnFixedUpdate()
        {
            if (!LevelRoot.Instance.IsStart) return;

            ChartPlayer.PlayUpdate(_musicRhythmManager.CurrentTime);
            noteContainer.CheckRelese(_musicRhythmManager.CurrentTime);
        }

        public NoteContainer NoteContainer => noteContainer;
        public ChartPlayer ChartPlayer => chartPlayer.Value;
        public int Count => noteContainer.Count;
        public SingleNote TryGetNearestNote(float currentTime) => noteContainer.TryGetNearestNote(currentTime);
        public void HitNote(SingleNote note)
        {
            OnPlayerHitOn?.Invoke();
            noteContainer.ReleseNote(note);
        }

        public LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;
    }
}
