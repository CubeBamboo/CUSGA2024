using CbUtils;
using Shuile.Core;
using Shuile.Core.Framework;
using Shuile.Gameplay;
using Shuile.Rhythm;
using UDebug = UnityEngine.Debug;

namespace Shuile.Rhythm.Runtime
{
    // play chart for single level
    // control enemy spawn and other event
    // it will auto play.
    public class LevelChartManager : MonoEntity
    {
        private MusicRhythmManager _musicRhythmManager;

        public bool isPlay = true;

        // chart part
        private ChartData chart;
        private ChartPlayer chartPlayer;

        private void Start()
        {
            _musicRhythmManager = this.GetSystem<MusicRhythmManager>();

            chart = LevelDataBinder.Instance.ChartData;
            chartPlayer = new ChartPlayer(chart);
            chartPlayer.OnNotePlay += (note, _) => note.Process();
        }

        private void FixedUpdate()
        {
#if UNITY_EDITOR
            if (!isPlay) return;
#endif
            chartPlayer.PlayUpdate(_musicRhythmManager.CurrentTime);
        }

        public override LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;
    }
}
