using CbUtils;
using Shuile.Rhythm;
using UDebug = UnityEngine.Debug;

namespace Shuile.Rhythm.Runtime
{
    // play chart for single level
    // control enemy spawn and other event
    // it will auto play.
    public class LevelChartManager : MonoSingletons<LevelChartManager>
    {
        public bool isPlay = true;

        // chart part
        private ChartData chart;
        private ChartPlayer chartPlayer;

        private void Start()
        {
            chart = LevelRoot.Instance.CurrentChart;
            chartPlayer = new ChartPlayer(chart);
            chartPlayer.OnNotePlay += note => note.Process();
        }

        private void FixedUpdate()
        {
            if (!isPlay) return;
            chartPlayer.PlayUpdate(MusicRhythmManager.Instance.CurrentTime);
        }
    }
}
