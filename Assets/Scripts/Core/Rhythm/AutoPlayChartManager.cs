using CbUtils;
using Shuile.Rhythm;
using UnityEngine;

namespace Shuile
{
    // manage auto play chart. (for someone like enemy or game ui animation)
    public class AutoPlayChartManager : MonoSingletons<AutoPlayChartManager>
    {
        // chart part
        private readonly ChartData chart = ChartDataCreator.CreatePlayerDefault();
        private ChartPlayer chartPlayer;

        /// <summary> call when a beat is hit </summary>
        public event System.Action OnRhythmHit;

        private void Start()
        {
            chartPlayer = new ChartPlayer(chart);
            chartPlayer.OnNotePlay += _ => OnRhythmHit?.Invoke();
        }

        private void FixedUpdate()
        {
            chartPlayer.PlayUpdate(MusicRhythmManager.Instance.CurrentTime);
        }
    }
}
