using CbUtils.Extension;
using DG.Tweening;
using Shuile.Chart;
using Shuile.Core.Global.Config;
using Shuile.MonoGadget;
using Shuile.ResourcesManagement.Loader;
using Shuile.Rhythm;
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
        [SerializeField] private float distanceUnit;

        [SerializeField] private float maxNegativeDeltaTime = 0.2f;

        private readonly ObjectPool<Graphic> _notePool;
        private readonly List<UINote> _uiNoteList;
        private LevelConfigSO _levelConfig;

        private Graphic _notePrefab;

        private PlayerChartManager _playerChartManager;
        private float _preDisplayTime;

        private ReadOnlyCollection<SingleNote> _renderNoteList;
        private Lazy<MusicTimeTweener> _timeTweener;

        public RhythmIndicator()
        {
            _notePool = new ObjectPool<Graphic>(
                () => Instantiate(_notePrefab.gameObject, transform).GetComponent<Graphic>(),
                g =>
                {
                    g.gameObject.SetActive(true);
                    g.enabled = true;
                },
                g => g.gameObject.SetActive(false),
                g => Destroy(g.gameObject));
            _uiNoteList = new List<UINote>(8);
        }

        public MusicTimeTweener TimeTweener => _timeTweener.Value;

        private float CurrentTime => TimeTweener.TweenTime;
        private float MissTolerance => ImmutableConfiguration.Instance.MissToleranceInSeconds;

        private void Start()
        {
            var resourcesLoader = LevelResourcesLoader.Instance;
            var sceneLocator = LevelScope.Interface;

            _playerChartManager = sceneLocator.GetImplementation<PlayerChartManager>();

            var preciseMusicPlayer = sceneLocator.GetImplementation<PreciseMusicPlayer>();
            _timeTweener =
                new Lazy<MusicTimeTweener>(() =>
                    preciseMusicPlayer.AudioPlayer.TargetSource.gameObject.GetOrAddComponent<MusicTimeTweener>());

            _levelConfig = resourcesLoader.SyncContext.levelConfig;
            _notePrefab = resourcesLoader.SyncContext.globalPrefabs.noteIndicator;
            _preDisplayTime = _levelConfig.playerNotePreShowTime;
            _playerChartManager.ChartPlayer.OnNotePlay += OnNote;
            _playerChartManager.OnPlayerHitOn += OnPlayerHit;
            _playerChartManager.NoteContainer.OnNoteAutoRelese += OnNoteNeedRelease;
        }

        private void Update()
        {
            for (var i = 0; i < _uiNoteList.Count;)
            {
                _uiNoteList[i++].UpdateView(TimeTweener.TweenTime, distanceUnit, _preDisplayTime, MissTolerance,
                    maxNegativeDeltaTime);
            }
        }

        private void OnDestroy()
        {
            if (LevelRoot.IsLevelActive)
            {
                _playerChartManager.ChartPlayer.OnNotePlay -= OnNote;
                _playerChartManager.OnPlayerHitOn -= OnPlayerHit;
                _playerChartManager.NoteContainer.OnNoteAutoRelese -= OnNoteNeedRelease;
            }

            _uiNoteList.Clear();
            _notePool.DestroyAll();
        }

        private void OnNoteNeedRelease(float time)
        {
            var uiNote = TryGetNearestNote();
            if (uiNote == null)
            {
                return;
            }

            ReleaseNote(uiNote);
        }

        private void OnPlayerHit()
        {
            if (_uiNoteList.Count == 0)
            {
                return;
            }

            var uiNote = TryGetNearestNote();
            if (uiNote == null)
            {
                return;
            }

            uiNote.isHit = true;
            uiNote.DoHitView(ReleaseNote);
        }

        private void OnNote(BaseNoteData noteData, float time)
        {
            var obj = _notePool.Get();
            var graphic = obj;
            graphic.color = graphic.color.With(a: 0f);
            _uiNoteList.Add(new UINote((RectTransform)obj.transform, graphic,
                noteData.GetNotePlayTime(LevelScope.Interface)));
        }

        private void ReleaseNote(UINote note)
        {
            _notePool.Release(note.graphic);
            _uiNoteList.UnorderedRemove(note);
        }

        //private float GetPlayTime(BaseNoteData note) => note.ToPlayTime() - preDisplayTime;

        private UINote TryGetNearestNote()
        {
            if (_uiNoteList.Count == 0)
            {
                return null;
            }

            return _uiNoteList.Min();
        }

        private class UINote : SingleNote
        {
            public readonly Graphic graphic;
            public readonly RectTransform transform;

            public bool isHit;

            public UINote(RectTransform transform, Graphic graphic, float targetTime) : base(targetTime)
            {
                this.transform = transform;
                this.graphic = graphic;
                isHit = false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateView(float time, float distanceUnit, float preDisplayTime, float MissTolerance,
                float maxNegativeDeltaTime = 0.2f)
            {
                if (isHit)
                {
                    return;
                }

                var delta = realTime - time;
                var waitForHit = delta > 0;

                transform.localPosition = transform.localPosition.With(distanceUnit * delta);

                if (!waitForHit && -delta > maxNegativeDeltaTime)
                {
                    graphic.enabled = false;
                    return;
                }

                var alpha = 1f - Mathf.Clamp01((delta - preDisplayTime + MissTolerance) / MissTolerance);
                graphic.color = Color.white.With(a: alpha);

                //float alpha = 1f - Mathf.Clamp01((delta < 0 ? -delta : delta - preDisplayTime + MissTolerance) / MissTolerance);
            }

            public void DoHitView(Action<UINote> onComplete)
            {
                isHit = true;
                graphic.DOFade(0, 0.2f).OnComplete(() => onComplete?.Invoke(this));
            }
        }
    }
}
