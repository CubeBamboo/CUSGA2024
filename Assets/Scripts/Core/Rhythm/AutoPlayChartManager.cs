using CbUtils;
using Cysharp.Threading.Tasks.Linq;
using Shuile.Core;
using Shuile.Gameplay;

namespace Shuile.Rhythm.Runtime
{
    // manage auto play chart. (for someone like enemy or game ui animation)
    public class AutoPlayChartManager : MonoSingletons<AutoPlayChartManager>
    {
        private IMusicRhythmManager _musicRhythmManager;

        // chart part
        private readonly ChartData chart = ChartDataCreator.CreatePlayerDefault();
        private ChartPlayer chartPlayer;

        /// <summary> call when a beat is hit </summary>
        public event System.Action OnRhythmHit;

        private System.Action onNextRhythm;
        /// <summary> will be called once when next beat is hit, and then it will be set to null </summary>
        public void OnNextRhythm(System.Action action)
            => onNextRhythm += action;

        private void Start()
        {
            _musicRhythmManager = GameApplication.ServiceLocator.GetService<IMusicRhythmManager>();
            chartPlayer = new ChartPlayer(chart);
            chartPlayer.OnNotePlay += (_, _) =>
            {
                OnRhythmHit?.Invoke();

                onNextRhythm?.Invoke();
                onNextRhythm = null;
            };
        }

        private void FixedUpdate()
        {
            chartPlayer.PlayUpdate(_musicRhythmManager.CurrentTime);
        }
    }
}
