using CbUtils;
using Shuile.Gameplay;
using UnityEngine;

namespace Shuile.Rhythm.Runtime
{

    // manage chart of player, convert chart to runtime note object noteContainer
    public class PlayerChartManager : MonoNonAutoSpawnSingletons<PlayerChartManager>
    {
        private readonly NoteContainer noteContainer = new();

        // chart part
        private readonly ChartData chart = ChartDataCreator.CreatePlayerDefault();
        private ChartPlayer chartPlayer;

        private LevelModel levelModel;

        private float notePreShowInterval = 0.4f;

        protected override void OnAwake()
        {
            levelModel = GameplayService.Interface.Get<LevelModel>();
            notePreShowInterval = LevelResources.Instance.levelConfig.playerNotePreShowTime;
        }

        private void Start()
        {
            chartPlayer = new ChartPlayer(chart,
                note => (note.targetTime * levelModel.BpmIntervalInSeconds - notePreShowInterval));
            chartPlayer.OnNotePlay += note => noteContainer.AddNote(note.targetTime);
        }

        private void FixedUpdate()
        {
            chartPlayer.PlayUpdate(MusicRhythmManager.Instance.CurrentTime);
            noteContainer.CheckRelese(MusicRhythmManager.Instance.CurrentTime);
        }

        public int Count => noteContainer.Count;
        public SingleNote TryGetNearestNote() => noteContainer.TryGetNearestNote();
        public void HitNote(SingleNote note) => noteContainer.ReleseNote(note);
    }
}
