// encoding: UTF-8 cp65001

using System.Collections.Generic;

namespace Shuile.Rhythm
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

        public int Count => notePool.Count;

        public void AddNote(float targetTime)
        {
            SingleNote note = notePool.Get();
            note.targetTime = targetTime;
            noteList.Add(note);
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
            noteList.Sort((a, b) => a.targetTime.CompareTo(b.targetTime)); //降序排序
            // 检查所有需要销毁的note
            while (noteList.Count > 0 && noteList[0].NeedRelese(currentTime, MusicRhythmManager.Instance.MissToleranceInSeconds))
            {
                notePool.Release(noteList[0]);
                noteList.RemoveAt(0);
            }
        }
    }

    // note object spawn in game runtime
    public class SingleNote
    {
        public float targetTime;

        public SingleNote(float targetTime)
        {
            this.targetTime = targetTime;
        }

        public bool NeedRelese(float currentTime, float missTolerance)
        {
            return currentTime - targetTime > missTolerance;
        }
    }

    public interface INoteContainer
    {
        void AddNote(float targetTime);
        void ReleseNote(SingleNote note);
        SingleNote TryGetNearestNote();
        void CheckRelese(float currentTime);
    }
}
