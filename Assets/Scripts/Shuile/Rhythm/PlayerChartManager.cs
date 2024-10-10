using Shuile.Chart;
using Shuile.Core.Global.Config;
using Shuile.Framework;
using Shuile.Gameplay;
using System;

namespace Shuile.Rhythm.Runtime
{
    // manage chart of player, convert chart to runtime note object noteContainer
    public class PlayerChartManager : BaseChartManager
    {
        // chart part
        private readonly ChartData _chart = ChartDataCreator.CreatePlayerDefault();
        private readonly LevelConfigSO _levelConfig;
        private readonly MusicRhythmManager _musicRhythmManager;
        private Lazy<ChartPlayer> _chartPlayer;

        private float _notePreShowInterval = 0.4f;

        public PlayerChartManager(RuntimeContext locator) : base(locator)
        {
            locator.Resolve(out _musicRhythmManager)
                .Resolve(out UnityEntryPointScheduler scheduler);

            scheduler.AddFixedUpdate(FixedTick);

            _levelConfig = GameApplication.BuiltInData.levelConfig;
            _notePreShowInterval = _levelConfig.playerNotePreShowTime;
            noteContainer = new NoteContainer();
            _chartPlayer = new Lazy<ChartPlayer>(() => new ChartPlayer(_chart,
                note => GetNotePlayTime(note) - _notePreShowInterval));
            ChartPlayer.OnNotePlay += (note, _) => noteContainer.AddNote(GetNotePlayTime(note));
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
