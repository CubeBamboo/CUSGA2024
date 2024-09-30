using Shuile.Chart;
using Shuile.Core.Framework.Unity;
using Shuile.Framework;
using Shuile.Rhythm.Runtime;
using System;

namespace Shuile.Rhythm
{
    // manage auto play chart. (for someone like enemy or game ui animation)
    public class AutoPlayChartManager : BaseChartManager, IStartable, IFixedTickable
    {
        // chart part
        private readonly ChartData _chart = ChartDataCreator.CreatePlayerDefault();
        private MusicRhythmManager _musicRhythmManager;
        private ChartPlayer _chartPlayer;

        private Action onNextRhythm;

        public AutoPlayChartManager(IGetableScope scope, ServiceLocator context) : base(scope)
        {
            context
                .Resolve(out _musicRhythmManager)
                .Resolve(out UnityEntryPointScheduler scheduler);

            scheduler.AddOnce(Start);
            scheduler.AddFixedUpdate(FixedTick);
            _chartPlayer = new ChartPlayer(_chart, this);
        }

        public void FixedTick()
        {
            _chartPlayer.PlayUpdate(_musicRhythmManager.CurrentTime);
        }

        public void Start()
        {
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
