// encoding: UTF-8 cp65001

using Shuile.Core.Configuration;
using Shuile.Gameplay;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Shuile.Rhythm.Runtime
{
    /* just a collection of SingleNote, no logic
     * only one note pool
     */
    public class NoteContainer : INoteContainer
    {
        // note to hit
        private ObjectPool<SingleNote> notePool = new ObjectPool<SingleNote>(
            ()=>new SingleNote(0f));

        private List<SingleNote> noteList = new();

        public event System.Action<float> OnNoteAutoRelese;
        public ReadOnlyCollection<SingleNote> OrderedNoteList => noteList.AsReadOnly();
        public int Count => notePool.Count;

        public SingleNote AddNote(float realTime)
        {
            SingleNote note = notePool.Get();
            note.realTime = realTime;
            noteList.Add(note);
            return note;
        }

        public void ReleseNote(SingleNote note)
        {
            noteList.Remove(note);
            notePool.Release(note);
        }

        public SingleNote TryGetNearestNote()
        {
            CheckRelese(MusicRhythmManager.Instance.CurrentTime);
            return noteList.Count != 0 ? noteList[0] : null;
        }

        public void CheckRelese(float currentTime)
        {
            if(noteList.Count == 0)
                return;
            noteList.Sort((a, b) => a.realTime.CompareTo(b.realTime)); //升序排序
            // 检查所有需要销毁的note
            while (noteList.Count > 0 && noteList[0].NeedRelese(currentTime, ImmutableConfiguration.Instance.MissToleranceInSeconds))
            {
                var time = noteList[0].realTime;
                notePool.Release(noteList[0]);
                noteList.RemoveAt(0);
                OnNoteAutoRelese?.Invoke(time);
            }
        }
    }

    // note object spawn in game runtime
    public class SingleNote : IComparable<SingleNote>
    {
        /// <summary>
        /// unit: in seconds
        /// </summary>
        public float realTime;

        public SingleNote(float targetTime)
        {
            this.realTime = targetTime;
        }

        public int CompareTo(SingleNote other)
            => (this.realTime - other.realTime) switch { < 0 => -1, > 0 => 1, _ => 0 };

        public bool NeedRelese(float currentTime, float missTolerance)
        {
            return currentTime > realTime && currentTime - realTime > missTolerance;
        }
    }

    public interface INoteContainer
    {
        SingleNote AddNote(float targetTime);
        void ReleseNote(SingleNote note);
        SingleNote TryGetNearestNote();
        void CheckRelese(float currentTime);
    }
}
