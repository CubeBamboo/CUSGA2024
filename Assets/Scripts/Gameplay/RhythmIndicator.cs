using Shuile.MonoGadget;
using Shuile.Rhythm.Runtime;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

using UObject = UnityEngine.Object;
using Shuile.ResourcesManagement.Loader;
using Shuile.Core.Configuration;
using System;
using Shuile.Core;

namespace Shuile.Gameplay
{
    public class RhythmIndicator : MonoBehaviour
    {
        private class UINote : SingleNote
        {
            public bool isHit;

            public RectTransform transform;
            public Graphic graphic;

            public bool isFadeOut = false;

            public UINote(RectTransform transform, Graphic graphic, float targetTime) : base(targetTime)
            {
                this.transform = transform;
                this.graphic = graphic;
                this.isHit = false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateView(float time, float distanceUnit, float preDisplayTime, float MissTolerance, float maxNegativeDeltaTime = 0.2f)
            {
                if (isHit) return;
                var delta = this.realTime - time;
                var waitForHit = delta > 0;

                this.transform.localPosition = this.transform.localPosition.With(x: distanceUnit * delta);

                if (!waitForHit && -delta > maxNegativeDeltaTime)
                {
                    this.graphic.enabled = false;
                    return;
                }

                float alpha = 1f - Mathf.Clamp01((delta - preDisplayTime + MissTolerance) / MissTolerance);
                this.graphic.color = Color.white.With(a: alpha);
                
                //float alpha = 1f - Mathf.Clamp01((delta < 0 ? -delta : delta - preDisplayTime + MissTolerance) / MissTolerance);
            }

            public void DoHitView(System.Action<UINote> onComplete)
            {
                isHit = true;
                this.graphic.DOFade(0, 0.2f).OnComplete(() => onComplete?.Invoke(this));
            }
        }

        private ReadOnlyCollection<SingleNote> renderNoteList;

        [SerializeField] private float distanceUnit;
        [SerializeField] private float maxNegativeDeltaTime = 0.2f;
        private float preDisplayTime;

        private Graphic notePrefab;

        private ObjectPool<Graphic> notePool;
        private List<UINote> uiNoteList;
        //private ChartPlayer chartPlayer;

        private IMusicRhythmManager _musicRhythmManager;
        private Lazy<MusicTimeTweener> timeTweener;

        public MusicTimeTweener TimeTweener => timeTweener.Value;

        private float CurrentTime => TimeTweener.TweenTime;
        private float MissTolerance => ImmutableConfiguration.Instance.MissToleranceInSeconds;

        public RhythmIndicator()
        {
            notePool = new ObjectPool<Graphic>(() => Instantiate(notePrefab.gameObject, transform).GetComponent<Graphic>(),
                g => { g.gameObject.SetActive(true); g.enabled = true; },
                g => g.gameObject.SetActive(false),
                g => UObject.Destroy(g.gameObject),
                8);
            uiNoteList = new(8);
        }

        private void Start()
        {
            _musicRhythmManager = GameApplication.ServiceLocator.GetService<IMusicRhythmManager>();
            timeTweener = new(() => _musicRhythmManager.PreciseMusicPlayer.gameObject.GetOrAddComponent<MusicTimeTweener>());

            notePrefab = LevelResourcesLoader.Instance.SyncContext.globalPrefabs.noteIndicator;
            preDisplayTime = PlayerChartManager.Instance.NotePreShowInterval;
            PlayerChartManager.Instance.ChartPlayer.OnNotePlay += OnNote;
            PlayerChartManager.Instance.OnPlayerHitOn += OnPlayerHit;
            PlayerChartManager.Instance.NoteContainer.OnNoteAutoRelese += OnNoteNeedRelese;
        }

        private void OnDestroy()
        {
            PlayerChartManager.TryAccessInstance(mgr =>
            {
                mgr.ChartPlayer.OnNotePlay -= OnNote;
                mgr.OnPlayerHitOn -= OnPlayerHit;
                mgr.NoteContainer.OnNoteAutoRelese -= OnNoteNeedRelese;
            });

            uiNoteList.Clear();
            notePool.DestroyAll();
        }

        private void Update()
        {
            for (int i = 0; i < uiNoteList.Count;)
            {
                uiNoteList[i++].UpdateView(TimeTweener.TweenTime, distanceUnit, preDisplayTime, MissTolerance, maxNegativeDeltaTime);
            }
        }

        private void OnNoteNeedRelese(float time)
        {
            var uiNote = TryGetNearestNote();
            if (uiNote == null) return;
            ReleseNote(uiNote);
        }

        private void OnPlayerHit()
        {
            if (uiNoteList.Count == 0) return;

            var uiNote = TryGetNearestNote();
            if (uiNote == null) return;

            uiNote.isHit = true;
            uiNote.DoHitView(n => ReleseNote(n));
        }

        private void OnNote(BaseNoteData noteData, float time)
        {
            var obj = notePool.Get();
            var graphic = obj;
            graphic.color = graphic.color.With(a: 0f);
            uiNoteList.Add(new ((RectTransform)obj.transform, graphic, noteData.ToPlayTime()));
        }

        private void ReleseNote(UINote note)
        {
            notePool.Release(note.graphic);
            uiNoteList.UnorderedRemove(note);
        }

        //private float GetPlayTime(BaseNoteData note) => note.ToPlayTime() - preDisplayTime;

        private UINote TryGetNearestNote()
        {
            if (uiNoteList.Count == 0) return null;
            return uiNoteList.Min();
        }
    }
}
