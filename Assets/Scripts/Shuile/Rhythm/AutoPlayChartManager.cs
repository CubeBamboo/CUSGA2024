using Shuile.Chart;
using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Gameplay;

namespace Shuile.Rhythm.Runtime
{
    // manage auto play chart. (for someone like enemy or game ui animation)
    public class AutoPlayChartManager : IStartable, IFixedTickable
    {
        private MusicRhythmManager _musicRhythmManager;

        // chart part
        private readonly ChartData chart = ChartDataCreator.CreatePlayerDefault();
        private ChartPlayer chartPlayer;

        /// <summary> call when a beat is hit </summary>
        public event System.Action OnRhythmHit;

        private System.Action onNextRhythm;
        /// <summary> will be called once when next beat is hit, and then it will be set to null </summary>
        public void OnNextRhythm(System.Action action)
            => onNextRhythm += action;

        public void Start()
        {
            var scope = LevelScope.Interface;
            _musicRhythmManager = MusicRhythmManager.Instance;
            chartPlayer = new ChartPlayer(chart, scope);
            chartPlayer.OnNotePlay += (_, _) =>
            {
                OnRhythmHit?.Invoke();

                onNextRhythm?.Invoke();
                onNextRhythm = null;
            };
        }

        public void FixedTick()
        {
            chartPlayer.PlayUpdate(_musicRhythmManager.CurrentTime);
        }
    }
}