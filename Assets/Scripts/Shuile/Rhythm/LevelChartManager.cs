using Shuile.Chart;
using Shuile.Core.Framework.Unity;
using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.Rhythm.Runtime;

namespace Shuile.Rhythm
{
    // play chart for single level
    // control enemy spawn and other event
    // it will auto play.
    public class LevelChartManager : BaseChartManager, IStartable, IFixedTickable
    {
        private MusicRhythmManager _musicRhythmManager;

        // chart part
        private readonly ChartData chart;
        private ChartPlayer chartPlayer;

        public bool isPlay = true;

        public LevelChartManager(IGetableScope scope, ServiceLocator context) : base(scope)
        {
            chart = LevelRoot.LevelContext.ChartData;

            context
                .Resolve(out _musicRhythmManager)
                .Resolve(out UnityEntryPointScheduler scheduler);

            scheduler.AddOnce(Start);
            scheduler.AddFixedUpdate(FixedTick);
        }

        public void FixedTick()
        {
#if UNITY_EDITOR
            if (!isPlay)
            {
                return;
            }
#endif
            chartPlayer.PlayUpdate(_musicRhythmManager.CurrentTime);
        }

        public void Start()
        {
            chartPlayer = new ChartPlayer(chart, this);
            chartPlayer.OnNotePlay += (note, _) => ProcessNote(note);
        }
    }
}
