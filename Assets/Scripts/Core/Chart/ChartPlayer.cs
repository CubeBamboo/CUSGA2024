// [WIP]

using CbUtils;

namespace Shuile.Rhythm
{
    /// <summary> provide play interface and then use monobehavior to update its time </summary>
    public class ChartPlayer
    {
        private readonly ChartData chart;
        private readonly float[] playTimeArray;
        private int nextNoteIndex = 0;

        /// <summary>
        /// call when it's time to play the note
        /// </summary>
        public event System.Action<NoteData> OnNotePlay = _ => { };

        // TODO: add note sort
        public ChartPlayer(ChartData chart, System.Func<NoteData, float> onPlayTimeConvert = null)
        {
            onPlayTimeConvert ??= note => note.targetTime * MusicRhythmManager.Instance.BpmInterval;

            this.chart = chart;
            playTimeArray = new float[chart.note.Length];
            for (int i = 0; i < chart.note.Length; i++)
            {
                playTimeArray[i] = onPlayTimeConvert.Invoke(chart.note[i]);
            }
        }

        /// <summary> call OnNotePlay when a note need to play </summary>
        /// <param name="time">update note state with it</param>
        public void PlayUpdate(float time)
        {
            if (nextNoteIndex >= playTimeArray.Length) return;

            // if next note time is less than current time, add note to noteContainer and trigger some event
            float nextNoteTime = playTimeArray[nextNoteIndex];
            if (time > nextNoteTime)
            {
                OnNotePlay.Invoke(chart.note[nextNoteIndex]);
                nextNoteIndex++;
            }
        }
    }

    /* // example
    public class PlayerChartManagerTest : MonoSingletons<PlayerChartManager>
    {
        private NoteContainer noteContainer = new();

        // chart part
        private readonly ChartData chart = ChartDataCreator.CreatePlayerDefault();
        private ChartPlayer chartPlayer;

        private void Start()
        {
            chartPlayer = new ChartPlayer(chart);
            chartPlayer.OnNotePlay += note => noteContainer.AddNote(note.targetTime);
        }

        private void FixedUpdate()
        {
            chartPlayer.PlayUpdate(MusicRhythmManager.Instance.CurrentTime);
        }

        public int Count => noteContainer.Count;
        public SingleNote TryGetNearestNote() => noteContainer.TryGetNearestNote();
        public void HitNote(SingleNote note) => noteContainer.ReleseNote(note);
    }*/
}
