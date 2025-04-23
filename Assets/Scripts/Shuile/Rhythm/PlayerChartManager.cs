using Shuile.Chart;
using Shuile.Core.Global.Config;
using Shuile.Framework;
using Shuile.Gameplay.Model;

namespace Shuile.Rhythm.Runtime
{
    // manage chart of player, convert chart to runtime note object noteContainer
    public class PlayerChartManager : BaseChartManager
    {
        private readonly ChartData _chart = ChartDataCreator.CreatePlayerDefault();
        private PlayerNoteList _noteList;
        private float _lastRhythmTime;

        private readonly LevelConfigSO _levelConfig;
        private readonly MusicRhythmManager _musicRhythmManager;

        public float LastHitNote { get; private set; }

        public PlayerNoteList NoteList => _noteList;

        public PlayerChartManager(RuntimeContext locator) : base(locator)
        {
            locator.Resolve(out _musicRhythmManager)
                .Resolve(out SingleLevelData singleLevelData)
                .Resolve(out UnityEntryPointScheduler scheduler);

            _levelConfig = GameApplication.BuiltInData.levelConfig;
            _chart.time = singleLevelData.ChartData.time;
            _noteList = new PlayerNoteList(_chart);

            scheduler.AddOnce(Start);
            scheduler.AddUpdate(Tick);
            scheduler.AddCallOnDestroy(() =>
            {
                _noteList.Dispose();
            });
        }

        private void Start()
        {
            _noteList.PlayStart();
        }

        private void Tick()
        {
            var delta = _musicRhythmManager.CurrentTime - _lastRhythmTime;
            _noteList.PlayTick(delta);
            _noteList.PlayerListTick(delta);
            _lastRhythmTime = _musicRhythmManager.CurrentTime;
        }

        // public event Action PlayerHitOn;

        public bool TryHitNoteNow()
        {
            var time = _musicRhythmManager.CurrentTime;
            var tolerance = _levelConfig.MissToleranceInSeconds;
            var note = _noteList.ActiveNotes.First;
            if (_noteList.TryHit(time, tolerance))
            {
                LastHitNote = note.Value.Time;
                // PlayerHitOn?.Invoke();
                return true;
            }

            return false;
        }
    }
}
