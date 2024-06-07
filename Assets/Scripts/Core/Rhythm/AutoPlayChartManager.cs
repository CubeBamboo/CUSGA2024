using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;

namespace Shuile.Rhythm.Runtime
{
    public class AutoPlayChartManagerUpdater : MonoEntity
    {
        private AutoPlayChartManager _autoPlayChartManager;
        private void Start()
        {
            _autoPlayChartManager = this.GetSystem<AutoPlayChartManager>();
            _autoPlayChartManager.OnStart();
        }
        private void FixedUpdate()
        {
            _autoPlayChartManager.OnFixedUpdate();
        }
        public override ModuleContainer GetModule() => GameApplication.Level;
    }

    // manage auto play chart. (for someone like enemy or game ui animation)
    public class AutoPlayChartManager : ISystem
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

        public void OnStart()
        {
            _musicRhythmManager = MusicRhythmManager.Instance;
            chartPlayer = new ChartPlayer(chart);
            chartPlayer.OnNotePlay += (_, _) =>
            {
                OnRhythmHit?.Invoke();

                onNextRhythm?.Invoke();
                onNextRhythm = null;
            };
        }

        public void OnFixedUpdate()
        {
            chartPlayer.PlayUpdate(_musicRhythmManager.CurrentTime);
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
