using Shuile.Chart;
using Shuile.Core.Framework.Unity;
using Shuile.Framework;
using Shuile.Gameplay.Model;
using Shuile.Rhythm.Runtime;
using System;

namespace Shuile.Rhythm
{
    // manage auto play chart. (for someone like enemy or game ui animation)
    public class AutoPlayChartManager : BaseChartManager, IStartable, IFixedTickable
    {
        // chart part
        private readonly ChartData _chart = ChartDataCreator.CreatePlayerDefault();
        private MusicRhythmManager _musicRhythmManager;

        private AutoPlayNoteList _noteList;
        private float _lastRhythmTime;

        private Action onNextRhythm;

        public AutoPlayChartManager(RuntimeContext context) : base(context)
        {
            context
                .Resolve(out _musicRhythmManager)
                .Resolve(out SingleLevelData singleLevelData)
                .Resolve(out UnityEntryPointScheduler scheduler);

            _chart.time = singleLevelData.ChartData.time;
            _noteList = new AutoPlayNoteList(_chart);
            _noteList.OnTickToNote += NoteListOnOnTickToNote;

            scheduler.AddFixedOnce(Start);
            scheduler.AddFixedUpdate(FixedTick);
        }

        private void NoteListOnOnTickToNote(AutoPlayNoteList.NoteData obj)
        {
            OnRhythmHit?.Invoke();

            onNextRhythm?.Invoke();
            onNextRhythm = null;
        }

        public void FixedTick()
        {
            _noteList.PlayTick(_musicRhythmManager.CurrentTime - _lastRhythmTime);
            _lastRhythmTime = _musicRhythmManager.CurrentTime;
        }

        public void Start()
        {
            _noteList.PlayStart();
        }

        /// <summary> call when a beat is hit </summary>
        public event Action OnRhythmHit;

        /// <summary> will be called once when next beat is hit, and then it will be set to null </summary>
        public void OnNextRhythm(Action action)
        {
            onNextRhythm += action;
        }
    }
}
