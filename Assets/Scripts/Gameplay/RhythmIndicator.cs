using DG.Tweening;
using Shuile.Framework;
using Shuile.MonoGadget;
using Shuile.Rhythm.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.UI;

namespace Shuile.Gameplay
{
    public class RhythmIndicator : MonoBehaviour
    {
        private class UINote : SingleNote
        {
            public bool isHit;

            public RectTransform transform;
            public Graphic graphic;

            public UINote(RectTransform transform, Graphic graphic, float targetTime) : base(targetTime)
            {
                this.transform = transform;
                this.graphic = graphic;
                this.isHit = false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateView(float time, float distanceUnit, float preDisplayTime, float MissTolerance)
            {
                if (isHit) return;
                var delta = this.realTime - time;
                var waitForHit = delta > 0;

                this.transform.localPosition = this.transform.localPosition.With(x: distanceUnit * delta);
                if (!waitForHit) return;

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
        private float preDisplayTime;

        private GameObject notePrefab;

        private ObjectPool<GameObject> notePool;
        private List<UINote> uiNoteList;
        //private ChartPlayer chartPlayer;

        private readonly CustomLoadObject<MusicTimeTweener> timeTweener
            = new(() => MusicRhythmManager.Instance.gameObject.GetOrAddComponent<MusicTimeTweener>());
        public MusicTimeTweener TimeTweener => timeTweener.Value;

        private float CurrentTime => TimeTweener.TweenTime;
        private float MissTolerance => GameplayService.Interface.LevelModel.MissToleranceInSeconds;

        public RhythmIndicator()
        {
            notePool = new ObjectPool<GameObject>(() => Instantiate(notePrefab, transform),
                ObjectPoolFuncStore.GameObjectGet,
                ObjectPoolFuncStore.GameObjectRelease,
                ObjectPoolFuncStore.GameObjectDestroy,
                8);
            uiNoteList = new(8);
        }

        private void Start()
        {
            notePrefab = LevelResources.Instance.globalPrefabs.noteIndicator;
            preDisplayTime = PlayerChartManager.Instance.NotePreShowInterval;
            PlayerChartManager.Instance.ChartPlayer.OnNotePlay += OnNote;
            PlayerChartManager.Instance.OnPlayerHitOn += OnPlayerHit;
            PlayerChartManager.Instance.NoteContainer.OnNoteAutoRelese += OnNoteNeedRelese;
        }

        private void OnDestroy()
        {
            PlayerChartManager.Instance.ChartPlayer.OnNotePlay -= OnNote;
            PlayerChartManager.Instance.OnPlayerHitOn -= OnPlayerHit;
            PlayerChartManager.Instance.NoteContainer.OnNoteAutoRelese -= OnNoteNeedRelese;

            uiNoteList.Clear();
            notePool.DestroyAll();
        }

        private void Update()
        {
            for (int i = 0; i < uiNoteList.Count;)
            {
                uiNoteList[i++].UpdateView(TimeTweener.TweenTime, distanceUnit, preDisplayTime, MissTolerance);
            }
        }

        private void OnNoteNeedRelese(float time)
        {
            var uiNote = TryGetNearestNote();
            if (uiNote == null) return;
            ReleseNote(uiNote);
            //uiNote.graphic.color = Color.red;
            //uiNote.graphic.DOFade(0f, 0.2f).OnComplete(() => ReleseNote(uiNote));
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
            var graphic = obj.GetComponent<Graphic>();
            graphic.color = graphic.color.With(a: 0f);
            uiNoteList.Add(new ((RectTransform)obj.transform, graphic, noteData.ToPlayTime()));
        }

        private void ReleseNote(UINote note)
        {
            notePool.Release(note.transform.gameObject);
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
