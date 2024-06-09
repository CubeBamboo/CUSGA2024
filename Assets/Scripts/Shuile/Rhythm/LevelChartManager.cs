using Shuile.Chart;
using Shuile.Core.Framework.Unity;
using Shuile.Gameplay;

namespace Shuile.Rhythm
{
    // play chart for single level
    // control enemy spawn and other event
    // it will auto play.
    public class LevelChartManager : IStartable, IFixedTickable
    {
        private MusicRhythmManager _musicRhythmManager;
        private NoteDataProcessor _noteDataProcessor;

        public bool isPlay = true;

        // chart part
        private ChartData chart;
        private ChartPlayer chartPlayer;

        public LevelChartManager(IGetableScope scope)
        {
            _noteDataProcessor = scope.Get<NoteDataProcessor>();
            chart = LevelRoot.LevelContext.ChartData;
            _musicRhythmManager = MusicRhythmManager.Instance;
        }

        public void Start()
        {
            chartPlayer = new ChartPlayer(chart, _noteDataProcessor);
            chartPlayer.OnNotePlay += (note, _) => note.ProcessNote(_noteDataProcessor);
        }

        public void FixedTick()
        {
#if UNITY_EDITOR
            if (!isPlay) return;
#endif
            chartPlayer.PlayUpdate(_musicRhythmManager.CurrentTime);
        }
    }
}
