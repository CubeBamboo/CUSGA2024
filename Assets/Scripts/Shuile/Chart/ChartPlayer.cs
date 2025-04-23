using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Shuile.Chart
{
    public interface INote
    {
        float Time { get; }
    }

    public abstract class BaseNoteList<T> : IEnumerable<T>, IDisposable where T : INote
    {
        private IEnumerator<T> _nextNoteEnumerator; // precise position for next note

        protected float _currentTime;

        public float CurrentTime => _currentTime;
        public T Current => _nextNoteEnumerator.Current;
        public bool MoveNext()
        {
            return _nextNoteEnumerator.MoveNext();
        }

        public bool Ticking { get; protected set; }
        public event Action<T> OnTickToNote; // real-time

        public void PlayStart()
        {
            _nextNoteEnumerator = GetEnumerator();
            Ticking = _nextNoteEnumerator.MoveNext();
        }

        // public void OnGUI()
        // {
        //     GUILayout.Label($"CurrentTime: {CurrentTime}");
        //     GUILayout.Label($"NextNote: {(Current?.Time.ToString() ?? "null")}");
        //     GUILayout.Label($"TickEnd: {Ticking}");
        // }

        public virtual void PlayTick(float delta)
        {
            if (!Ticking) return;

            _currentTime += delta;
            if (_currentTime > _nextNoteEnumerator.Current.Time)
            {
                OnTickToNote?.Invoke(_nextNoteEnumerator.Current);
                Ticking = _nextNoteEnumerator.MoveNext();
            }
        }

        public abstract IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            _nextNoteEnumerator?.Dispose();
        }
    }

    public abstract class PreNoteList<T> : BaseNoteList<T> where T : INote
    {
        protected float preInterval; // 1.5s before the note

        public event Action<T> OnTickToPreNote; // note need to be process in advance

        protected PreNoteList(float preInterval)
        {
            this.preInterval = preInterval;
        }

        public override void PlayTick(float delta)
        {
            if (!Ticking) return;

            _currentTime += delta;
            if (_currentTime > Current.Time - preInterval)
            {
                var current = Current;
                OnTickToPreNote?.Invoke(current);
                Ticking = MoveNext();
            }
        }
    }

    public class LevelNoteList : PreNoteList<LevelNoteList.NoteData>
    {
        public struct NoteData : INote
        {
            public int type;
            public float time;

            public float Time => time;
        }

        private List<NoteData> _realTimeList;

        public LevelNoteList(ChartData chartData) : base(0f)
        {
            var chart = chartData;
            var bpm = chart.time[0].bpm;
            var offset = chart.time[0].offset * 0.001f;
            preInterval = Laser.InTime * (60 / bpm) + offset; // so complex so calculate here

            _realTimeList = chart.note.Select(x =>
            {
                return new NoteData()
                {
                    time = x.rhythmTime * (60 / bpm) + offset,
                    type = x.GetType().Name switch
                    {
                        nameof(SpawnLaserNoteData) => 0,
                        nameof(SpawnSingleEnemyNoteData) => 1,
                        _ => -1
                    }
                };
            }).ToList();
            _realTimeList.Sort((a, b) =>
            {
                var del = a.time - b.time;
                return del == 0 ? 0 : del > 0 ? 1 : -1;
            });
        }

        public override IEnumerator<NoteData> GetEnumerator()
        {
            return _realTimeList.GetEnumerator();
        }
    }

    public class AutoPlayNoteList : BaseNoteList<AutoPlayNoteList.NoteData>
    {
        public struct NoteData : INote
        {
            public float time;
            public float Time => time;
        }

        private float bpm;
        private float offset;

        private int _count = 0;

        public AutoPlayNoteList(ChartData chartData)
        {
            bpm = chartData.time[0].bpm;
            offset = chartData.time[0].offset * 0.001f;
        }

        public bool PlayEnd { get; set; }

        public override IEnumerator<NoteData> GetEnumerator()
        {
            while (!PlayEnd)
            {
                yield return new NoteData() { time = (_count++) * (60 / bpm) + offset };
            }
        }
    }

    // no need to tick
    public class PlayerNoteList : PreNoteList<PlayerNoteList.NoteData>
    {
        public struct NoteData : INote
        {
            public float time;

            public float Time => time;
        }

        private float bpm;
        private float offset;
        // private float currentTime;

        private int _count = 0;

        private Stack<LinkedListNode<NoteData>> _nodePool = new Stack<LinkedListNode<NoteData>>();

        public LinkedList<NoteData> ActiveNotes { get; } = new LinkedList<NoteData>();

        public event Action<NoteData> ActiveNoteNewEnter, ActiveNoteDiscard, ActiveNoteHit;

        public PlayerNoteList(ChartData chartData) : base(1.5f)
        {
            bpm = chartData.time[0].bpm;
            offset = chartData.time[0].offset * 0.001f;

            OnTickToPreNote += note =>
            {
                var node = GetNode(note); // gc optimize
                ActiveNotes.AddLast(node);
                // ActiveNotes.AddLast(NextNote);

                ActiveNoteNewEnter?.Invoke(Current);
            };
        }

        private LinkedListNode<NoteData> GetNode(NoteData note)
        {
            if (_nodePool.TryPop(out var node))
            {
                node.Value = note;
                return node;
            }
            else
            {
                return new LinkedListNode<NoteData>(note);
            }
        }

        private void ReleaseNode(LinkedListNode<NoteData> node)
        {
            _nodePool.Push(node);
        }

        public void PlayerListTick(float delta)
        {
            if (!Ticking) return;
            if (ActiveNotes.First == null) return;

            if (_currentTime > ActiveNotes.First.Value.time + 0.3f)
            {
                var first = ActiveNotes.First;
                ActiveNotes.RemoveFirst();
                ReleaseNode(first);
                ActiveNoteDiscard?.Invoke(first.Value);
            }
        }

        public bool TryHit(float time, float tolerance)
        {
            if (ActiveNotes.First == null)
            {
                return false;
            }

            if (Mathf.Abs(ActiveNotes.First.Value.Time - time) < tolerance)
            {
                var first = ActiveNotes.First.Value;
                ActiveNotes.RemoveFirst();
                ActiveNoteHit?.Invoke(first);
                return true;
            }

            return false;
        }

        public bool PlayEnd { get; set; }

        public override IEnumerator<NoteData> GetEnumerator()
        {
            while (!PlayEnd)
            {
                yield return new NoteData() { time = (_count++) * (60 / bpm) + offset };
            }
        }
    }
}

/*public float PeekNearest(float time, float tolerance)
{
    if (NextNote.Time < time) // less
    {
        var nearestLess = NextNote.Time;
        if (time - NextNote.Time > tolerance) // soooo far, use a nearer one
        {
            var bpmInterval = 60 / bpm;
            var delta = time - (NextNote.Time - offset);
            nearestLess = NextNote.Time + (int)(delta / bpmInterval) * bpmInterval;
        }

        if (time - nearestLess < tolerance)
        {
            return nearestLess;
        }
        else
        {
            return nearestLess + 60 / bpm;
        }
    }
    else
    {
        return time;
    }
}

public bool MoveToNearest(float time, float tolerance)
{
    var mov = true;
    while (mov && NextNote.Time < time && time - NextNote.Time > tolerance)
    {
        mov = MoveNext();
    }

    return mov;
}

public float GetNearest(float time, float tolerance)
{
    MoveToNearest(time, tolerance);
    return NextNote.Time;
}*/
