using Shuile.Chart;
using Shuile.Core.Framework.Unity;
using Shuile.Core.Global.Config;
using Shuile.Gameplay;
using Shuile.ResourcesManagement.Loader;
using System;

namespace Shuile.Rhythm.Runtime
{
    // manage chart of player, convert chart to runtime note object noteContainer
    public class PlayerChartManager : IInitializeable, IFixedTickable
    {
        // chart part
        private readonly ChartData _chart = ChartDataCreator.CreatePlayerDefault();
        private readonly LevelConfigSO _levelConfig;
        private readonly MusicRhythmManager _musicRhythmManager;
        private readonly NoteDataProcessor _noteDataProcessor;
        private Lazy<ChartPlayer> _chartPlayer;

        private float _notePreShowInterval = 0.4f;

        public PlayerChartManager(IGetableScope scope)
        {
            _musicRhythmManager = scope.GetImplementation<MusicRhythmManager>();
            ;
            _noteDataProcessor = scope.GetImplementation<NoteDataProcessor>();

            _levelConfig = LevelResourcesLoader.Instance.SyncContext.levelConfig;
        }

        public NoteContainer noteContainer { get; private set; }

        public NoteContainer NoteContainer => noteContainer;
        public ChartPlayer ChartPlayer => _chartPlayer.Value;
        public int Count => noteContainer.Count;

        public void FixedTick()
        {
            if (!LevelRoot.Instance.IsStart)
            {
                return;
            }

            ChartPlayer.PlayUpdate(_musicRhythmManager.CurrentTime);
            noteContainer.CheckRelease(_musicRhythmManager.CurrentTime);
        }

        public void Initialize()
        {
            _notePreShowInterval = _levelConfig.playerNotePreShowTime;

            noteContainer = new NoteContainer();
            _chartPlayer = new Lazy<ChartPlayer>(() => new ChartPlayer(_chart,
                note => note.GetNotePlayTime(_noteDataProcessor) - _notePreShowInterval));
            ChartPlayer.OnNotePlay += (note, _) => noteContainer.AddNote(note.GetNotePlayTime(_noteDataProcessor));
        }

        public event Action OnPlayerHitOn;

        public SingleNote TryGetNearestNote(float currentTime)
        {
            return noteContainer.TryGetNearestNote(currentTime);
        }

        public void HitNote(SingleNote note)
        {
            OnPlayerHitOn?.Invoke();
            noteContainer.ReleaseNote(note);
        }
    }
}
