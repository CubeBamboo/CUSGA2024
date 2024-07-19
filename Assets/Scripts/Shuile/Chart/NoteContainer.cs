using Shuile.Core.Global.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Shuile.Chart
{
    /* just a collection of SingleNote, no logic
     * only one note pool
     */
    public class NoteContainer : INoteContainer
    {
        private readonly List<SingleNote> _noteList = new();

        // note to hit
        private readonly ObjectPool<SingleNote> _notePool = new(
            () => new SingleNote(0f));

        public ReadOnlyCollection<SingleNote> OrderedNoteList => _noteList.AsReadOnly();
        public int Count => _notePool.Count;

        public SingleNote AddNote(float realTime)
        {
            var note = _notePool.Get();
            note.realTime = realTime;
            _noteList.Add(note);
            return note;
        }

        public void ReleaseNote(SingleNote note)
        {
            _noteList.Remove(note);
            _notePool.Release(note);
        }

        public SingleNote TryGetNearestNote(float currentTime)
        {
            CheckRelease(currentTime);
            return _noteList.Count != 0 ? _noteList[0] : null;
        }

        public void CheckRelease(float currentTime)
        {
            if (_noteList.Count == 0)
            {
                return;
            }

            _noteList.Sort((a, b) => a.realTime.CompareTo(b.realTime)); //升序排序
            // 检查所有需要销毁的note
            while (_noteList.Count > 0 &&
                   _noteList[0].NeedRelease(currentTime, ImmutableConfiguration.Instance.MissToleranceInSeconds))
            {
                var time = _noteList[0].realTime;
                _notePool.Release(_noteList[0]);
                _noteList.RemoveAt(0);
                OnNoteAutoRelese?.Invoke(time);
            }
        }

        public event Action<float> OnNoteAutoRelese;
    }

    // note object spawn in game runtime
    public class SingleNote : IComparable<SingleNote>
    {
        /// <summary>
        ///     unit: in seconds
        /// </summary>
        public float realTime;

        public SingleNote(float targetTime)
        {
            realTime = targetTime;
        }

        public int CompareTo(SingleNote other)
        {
            return (realTime - other.realTime) switch { < 0 => -1, > 0 => 1, _ => 0 };
        }

        public bool NeedRelease(float currentTime, float missTolerance)
        {
            return currentTime > realTime && currentTime - realTime > missTolerance;
        }
    }

    public interface INoteContainer
    {
        SingleNote AddNote(float targetTime);
        void ReleaseNote(SingleNote note);
        SingleNote TryGetNearestNote(float currentTime);
        void CheckRelease(float currentTime);
    }
}
