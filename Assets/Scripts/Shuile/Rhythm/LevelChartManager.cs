using Shuile.Chart;
using Shuile.Framework;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Manager;
using Shuile.Gameplay.Model;
using Shuile.Rhythm.Runtime;
using System;
using UnityEngine;

namespace Shuile.Rhythm
{
    // play chart for single level
    // control enemy spawn and other event
    // it will auto play.
    public class LevelChartManager : BaseChartManager
    {
        private readonly LevelEntityManager _entityManager;
        private readonly LevelZoneManager _levelZoneManager;

        private MusicRhythmManager _musicRhythmManager;

        private float _lastRhythmTime;
        private LevelNoteList _noteList;

        public bool isPlay = true;

        public LevelChartManager(RuntimeContext context) : base(context)
        {
            context
                .Resolve(out _musicRhythmManager)
                .Resolve(out SingleLevelData levelContext)
                .Resolve(out _entityManager)
                .Resolve(out _levelZoneManager)
                .Resolve(out UnityEntryPointScheduler scheduler);

            var chart = levelContext.ChartData;
            _noteList = new LevelNoteList(chart);
            _noteList.OnTickToPreNote += NoteListOnOnTickToNote;

            scheduler.AddOnce(Start);
            scheduler.AddUpdate(Tick);
            scheduler.AddCallOnDestroy(() =>
            {
                _noteList.Dispose();
            });
        }

        private void NoteListOnOnTickToNote(LevelNoteList.NoteData obj)
        {
            switch (obj.type)
            {
                case 0:
                    var inst = _entityManager.EntityFactory.SpawnLaser();
                    inst.transform.position = _levelZoneManager.RandomValidPosition();
                    break;
                case 1:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentException();
            }
        }

        public void Start()
        {
            _noteList.PlayStart();
            _lastRhythmTime = _musicRhythmManager.CurrentTime;
        }

        public void Tick()
        {
#if UNITY_EDITOR
            if (!isPlay)
            {
                return;
            }
#endif

            _noteList.PlayTick(_musicRhythmManager.CurrentTime - _lastRhythmTime);
            _lastRhythmTime = _musicRhythmManager.CurrentTime;
        }
    }
}
