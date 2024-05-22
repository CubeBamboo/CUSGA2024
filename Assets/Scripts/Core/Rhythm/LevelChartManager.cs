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
            chart = LevelDataBinder.Instance.ChartData;
            chartPlayer = new ChartPlayer(chart);
            chartPlayer.OnNotePlay += (note, _) => note.Process();
        }

        private void FixedUpdate()
        {
#if UNITY_EDITOR
            if (!isPlay) return;
#endif
            chartPlayer.PlayUpdate(MusicRhythmManager.Instance.CurrentTime);
        }
    }
}
