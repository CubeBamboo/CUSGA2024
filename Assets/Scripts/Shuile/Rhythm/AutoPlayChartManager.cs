using Shuile.Chart;
using Shuile.Core.Framework.Unity;
using System;

namespace Shuile.Rhythm
{
    // manage auto play chart. (for someone like enemy or game ui animation)
    public class AutoPlayChartManager : IStartable, IFixedTickable
    {
        // chart part
        private readonly ChartData _chart = ChartDataCreator.CreatePlayerDefault();
        private readonly MusicRhythmManager _musicRhythmManager;
        private readonly NoteDataProcessor _noteDataProcessor;
        private ChartPlayer _chartPlayer;

        private Action onNextRhythm;

        public AutoPlayChartManager(IGetableScope scope)
        {
            _musicRhythmManager = scope.GetImplementation<MusicRhythmManager>();
            _noteDataProcessor = scope.GetImplementation<NoteDataProcessor>();
        }

        public void FixedTick()
        {
            _chartPlayer.PlayUpdate(_musicRhythmManager.CurrentTime);
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

        /// <summary> call when a beat is hit </summary>
        public event Action OnRhythmHit;

        /// <summary> will be called once when next beat is hit, and then it will be set to null </summary>
        public void OnNextRhythm(Action action)
        {
            onNextRhythm += action;
        }
    }
}
