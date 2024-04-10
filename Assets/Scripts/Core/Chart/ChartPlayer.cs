// [WIP]

using CbUtils;
using System.Collections.Generic;

namespace Shuile.Rhythm.Runtime
{
    /// <summary> provide play interface and then use monobehavior to update its time </summary>
    public class ChartPlayer
    {
        private struct PlayTimeData
        {
            public float playTime;
            public int originNoteIndex;
        }

        private readonly ChartData chart;
        private readonly List<PlayTimeData> playTimeArray;

        private int nextNoteIndex = 0;

        /// <summary>
        /// call when it's time to play the note
        /// </summary>
        public event System.Action<BaseNoteData> OnNotePlay = _ => { };

        public ChartPlayer(ChartData chart, System.Func<BaseNoteData, float> onPlayTimeConvert = null)
        {
            onPlayTimeConvert ??= note => note.ToPlayTime();

            this.chart = chart;
            // convert target time (music beat) to play time (show in screen)
            playTimeArray = new List<PlayTimeData>(chart.note.Length);
            playTimeArray.Clear();
            for (int i = 0; i < chart.note.Length; i++)
            {
                playTimeArray.Add(new PlayTimeData()
                {
                    playTime = onPlayTimeConvert.Invoke(chart.note[i]),
                    originNoteIndex = i
                });
            }
            playTimeArray.Sort((a, b) => a.playTime.CompareTo(b.playTime));
        }

        /// <summary> call OnNotePlay when a note need to play </summary>
        /// <param name="time">update note state with it</param>
        public void PlayUpdate(float time)
        {
            if (nextNoteIndex >= playTimeArray.Count) return;

            // if next note time is less than current time, add note to noteContainer and trigger some event
            float nextNoteTime = playTimeArray[nextNoteIndex].playTime;
            if (time > nextNoteTime)
            {
                OnNotePlay.Invoke(chart.note[playTimeArray[nextNoteIndex].originNoteIndex]);
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
