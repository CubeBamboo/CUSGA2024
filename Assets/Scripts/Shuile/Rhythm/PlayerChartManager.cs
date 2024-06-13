using Shuile.Chart;
using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Core.Global.Config;
using Shuile.Gameplay;
using Shuile.ResourcesManagement.Loader;

namespace Shuile.Rhythm.Runtime
{
    // manage chart of player, convert chart to runtime note object noteContainer
    public class PlayerChartManager : ISystem, IInitializeable, IFixedTickable
    {
        private System.Lazy<ChartPlayer> _chartPlayer;

        // chart part
        private readonly ChartData _chart = ChartDataCreator.CreatePlayerDefault();

        private float _notePreShowInterval = 0.4f;
        private readonly LevelConfigSO _levelConfig;
        private readonly MusicRhythmManager _musicRhythmManager;
        private readonly NoteDataProcessor _noteDataProcessor;

        public event System.Action OnPlayerHitOn;
        public NoteContainer noteContainer { get; private set;}

        public PlayerChartManager(IGetableScope scope)
        {
            _musicRhythmManager = scope.GetImplementation<MusicRhythmManager>();;
            _noteDataProcessor = scope.GetImplementation<NoteDataProcessor>();
         
            _levelConfig = LevelResourcesLoader.Instance.SyncContext.levelConfig;
        }

        public void Initialize()
        {
            _notePreShowInterval = _levelConfig.playerNotePreShowTime;

            noteContainer = new();
            _chartPlayer = new(() => new ChartPlayer(_chart,
                note => note.GetNotePlayTime(_noteDataProcessor) - _notePreShowInterval));
            ChartPlayer.OnNotePlay += (note, _) => noteContainer.AddNote(note.GetNotePlayTime(_noteDataProcessor));
        }

        public void FixedTick()
        {
            if (!LevelRoot.Instance.IsStart) return;

            ChartPlayer.PlayUpdate(_musicRhythmManager.CurrentTime);
            noteContainer.CheckRelease(_musicRhythmManager.CurrentTime);
        }

        public NoteContainer NoteContainer => noteContainer;
        public ChartPlayer ChartPlayer => _chartPlayer.Value;
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
