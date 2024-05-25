using CbUtils;
using CbUtils.Kits.Tasks;
using Shuile.Core.Configuration;
using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.Gameplay.Event;
using Shuile.ResourcesManagement.Loader;
using Shuile.Root;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UnityEngine;

namespace Shuile.Rhythm.Runtime
{

    // manage chart of player, convert chart to runtime note object noteContainer
    public class PlayerChartManager : MonoSingletons<PlayerChartManager>
    {
        private readonly NoteContainer noteContainer = new();

        // chart part
        private readonly ChartData chart = ChartDataCreator.CreatePlayerDefault();
        private CustomLoadObject<ChartPlayer> chartPlayer;

        private LevelModel levelModel;

        private float notePreShowInterval = 0.4f;
        public System.Action OnPlayerHitOn;
        private LevelConfigSO _levelConfig;

        public float NotePreShowInterval => notePreShowInterval;
        public NoteContainer NoteContainer => noteContainer;
        public ChartPlayer ChartPlayer => chartPlayer.Value;
        public ReadOnlyCollection<SingleNote> OrderedNoteList => noteContainer.OrderedNoteList;

        protected override void OnAwake()
        {
            levelModel = GameplayService.Interface.Get<LevelModel>();
            InitilizeResources();
            notePreShowInterval = _levelConfig.playerNotePreShowTime;
            chartPlayer = new(() => new ChartPlayer(chart,
                note => note.GetRealTime() - notePreShowInterval));
            ChartPlayer.OnNotePlay += (note, _) => noteContainer.AddNote(note.GetRealTime());
        }

        private void InitilizeResources()
        {
            _levelConfig = LevelResourcesLoader.Instance.SyncContext.levelConfig;
        }

        private void FixedUpdate()
        {
            if (!LevelRoot.Instance.IsStart) return;

            ChartPlayer.PlayUpdate(MusicRhythmManager.Instance.CurrentTime);
            noteContainer.CheckRelese(MusicRhythmManager.Instance.CurrentTime);
        }

        public int Count => noteContainer.Count;
        public SingleNote TryGetNearestNote() => noteContainer.TryGetNearestNote();
        public void HitNote(SingleNote note) => noteContainer.ReleseNote(note);
    }

    public static class PlayerChartManagerExtension
    {
        public static bool TryHit(this PlayerChartManager self, float inputTime, out float hitOffset)
        {
            float missTolerance = ImmutableConfiguration.Instance.MissToleranceInSeconds;

            // get the nearest note's time and judge
            hitOffset = float.NaN;
            SingleNote targetNote = self.TryGetNearestNote();
            if (targetNote == null)
                return false;

            bool ret = Mathf.Abs(inputTime - targetNote.realTime) < missTolerance;
            if (ret)
            {
                self.HitNote(targetNote);
                hitOffset = inputTime - targetNote.realTime;
                self.OnPlayerHitOn?.Invoke();
            }
            return ret;
        }
    }
}
