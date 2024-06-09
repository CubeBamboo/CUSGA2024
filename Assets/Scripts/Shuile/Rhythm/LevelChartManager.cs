using Shuile.Chart;
using Shuile.Core.Framework.Unity;
using Shuile.Gameplay;
using Shuile.Rhythm.Runtime;

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

        public void Start()
        {
            var scope = LevelScope.Interface;
            _musicRhythmManager = MusicRhythmManager.Instance;
            _noteDataProcessor = scope.Get<NoteDataProcessor>();
            
            chart = LevelRoot.LevelContext.ChartData;
            chartPlayer = new ChartPlayer(chart, scope);
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
