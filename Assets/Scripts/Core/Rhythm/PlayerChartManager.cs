using CbUtils;
using UnityEngine;

namespace Shuile.Rhythm
{

    // manage chart of player, convert chart to runtime note object noteContainer
    public class PlayerChartManager : MonoSingletons<PlayerChartManager>
    {
        private readonly NoteContainer noteContainer = new();

        // chart part
        private readonly ChartData chart = ChartDataCreator.CreatePlayerDefault();
        private ChartPlayer chartPlayer;

        [SerializeField] private float notePreShowInterval = 0.4f;

        private void Start()
        {
            chartPlayer = new ChartPlayer(chart,
                note => (note.targetTime * MusicRhythmManager.Instance.BpmInterval - notePreShowInterval));
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
