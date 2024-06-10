using Shuile.Chart;
using Shuile.Core.Framework.Unity;

namespace Shuile.Rhythm
{
    // manage auto play chart. (for someone like enemy or game ui animation)
    public class AutoPlayChartManager : IStartable, IFixedTickable
    {
        private readonly MusicRhythmManager _musicRhythmManager;
        private readonly NoteDataProcessor _noteDataProcessor;

        // chart part
        private readonly ChartData _chart = ChartDataCreator.CreatePlayerDefault();
        private ChartPlayer _chartPlayer;

        /// <summary> call when a beat is hit </summary>
        public event System.Action OnRhythmHit;

        private System.Action onNextRhythm;
        /// <summary> will be called once when next beat is hit, and then it will be set to null </summary>
        public void OnNextRhythm(System.Action action)
            => onNextRhythm += action;

        public AutoPlayChartManager(IGetableScope scope)
        {
            _musicRhythmManager = MusicRhythmManager.Instance;
            _noteDataProcessor = scope.GetImplementation<NoteDataProcessor>();
        }

        public void Start()
        {
            _chartPlayer = new ChartPlayer(_chart, _noteDataProcessor);
            _chartPlayer.OnNotePlay += (_, _) =>
            {
                OnRhythmHit?.Invoke();

                onNextRhythm?.Invoke();
                onNextRhythm = null;
            };
        }

        public void FixedTick()
        {
            _chartPlayer.PlayUpdate(_musicRhythmManager.CurrentTime);
        }
    }
}
