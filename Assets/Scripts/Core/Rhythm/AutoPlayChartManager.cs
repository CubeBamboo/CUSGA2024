using CbUtils;
using Shuile.Gameplay;

namespace Shuile.Rhythm.Runtime
{
    // manage auto play chart. (for someone like enemy or game ui animation)
    public class AutoPlayChartManager : MonoNonAutoSpawnSingletons<AutoPlayChartManager>
    {
        // chart part
        private readonly ChartData chart = ChartDataCreator.CreatePlayerDefault();
        private ChartPlayer chartPlayer;
        private LevelModel levelModel;

        /// <summary> call when a beat is hit </summary>
        //public event System.Action OnRhythmHit;

        private System.Action onNextRhythm;
        /// <summary> will be called once when next beat is hit, and then it will be set to null </summary>
        public void OnNextRhythm(System.Action action)
            => onNextRhythm += action;
        //public void OnNextRhythm(UniTask.Action action)
        //    => onNextRhythm += action;

        private void Start()
        {
            levelModel = GameplayService.Interface.Get<LevelModel>();

            chartPlayer = new ChartPlayer(chart);
            chartPlayer.OnNotePlay += _ =>
            {
                levelModel.onRhythmHit?.Invoke();
                //OnRhythmHit?.Invoke();

                onNextRhythm?.Invoke();
                onNextRhythm = null;
            };
        }

        private void FixedUpdate()
        {
            chartPlayer.PlayUpdate(MusicRhythmManager.Instance.CurrentTime);
        }
    }
}
