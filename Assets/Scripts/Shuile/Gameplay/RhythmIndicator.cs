using CbUtils.Extension;
using DG.Tweening;
using Shuile.Chart;
using Shuile.Core.Global.Config;
using Shuile.Framework;
using Shuile.MonoGadget;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile.Gameplay
{
    public class RhythmIndicator : MonoBehaviour
    {
        [SerializeField] private float distanceUnit;

        [SerializeField] private float maxNegativeDeltaTime = 0.2f;

        private ObjectPool<Graphic> _notePool;
        private UINoteList _uiNoteListControl;
        private LevelConfigSO _levelConfig;

        private Graphic _notePrefab;

        private PlayerChartManager _playerChartManager;

        private ReadOnlyCollection<SingleNote> _renderNoteList;
        private MusicTimeTweener _timeTweener;

        private float CurrentTime => _timeTweener.TweenTime;
        private float MissTolerance => GameApplication.BuiltInData.levelConfig.MissToleranceInSeconds;

        private void Awake()
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
        }

        private void Start()
        {
            var sceneContext = SceneContainer.Instance.Context;
            sceneContext.Resolve(out GamePlayScene playScene);
            if (playScene.TryGetPlayer(out var player))
            {
                player.Context.Resolve(out _playerChartManager);
            }
            else
            {
                Debug.LogWarning("Player not found, RhythmIndicator will not work");
                enabled = false;
                return;
            }

            var preciseMusicPlayer = sceneContext.GetImplementation<PreciseMusicPlayer>();
            _timeTweener =
                preciseMusicPlayer.AudioPlayer.TargetSource.gameObject.GetOrAddComponent<MusicTimeTweener>();

            var builtInData = GameApplication.BuiltInData;
            _levelConfig = builtInData.levelConfig;
            _notePrefab = builtInData.globalPrefabs.noteIndicator;

            _uiNoteListControl = new UINoteList(_timeTweener, distanceUnit);

            // $$key
            _playerChartManager.NoteList.ActiveNoteNewEnter += NoteListOnActiveNoteNewEnter;
            _playerChartManager.NoteList.ActiveNoteHit += NoteListOnActiveNoteHit;
            _playerChartManager.NoteList.ActiveNoteDiscard += NoteListOnActiveNoteDiscard;
        }

        private void Update()
        {
            _uiNoteListControl.Tick();

#if UNITY_EDITOR
            _uiNoteListControl.RefreshParams(distanceUnit);
#endif
        }

        private void OnDestroy()
        {
            _playerChartManager.NoteList.ActiveNoteNewEnter -= NoteListOnActiveNoteNewEnter;
            _playerChartManager.NoteList.ActiveNoteHit -= NoteListOnActiveNoteHit;
            _playerChartManager.NoteList.ActiveNoteDiscard -= NoteListOnActiveNoteDiscard;

            _notePool.DestroyAll();
        }

        private void NoteListOnActiveNoteNewEnter(PlayerNoteList.NoteData obj)
        {
            var graphic = Instantiate(_notePrefab.gameObject, transform).GetComponent<Graphic>();
            graphic.color = graphic.color.With(a: 0f);
            _uiNoteListControl.AddLast(graphic, obj.Time);
        }

        private void NoteListOnActiveNoteHit(PlayerNoteList.NoteData obj)
        {
            _uiNoteListControl.HitFirst();
        }

        private void NoteListOnActiveNoteDiscard(PlayerNoteList.NoteData obj)
        {
            _uiNoteListControl.RemoveFirst();
        }

        //private float GetPlayTime(BaseNoteData note) => note.ToPlayTime() - preDisplayTime;

        // handle a list of notes gameObject (mono behaviour)
        private class UINoteList
        {
            private float distanceUnit;
            private Queue<UINoteProxy> _uiList;

            // shit
            private MusicTimeTweener _timeTweener;

            public UINoteList(MusicTimeTweener timeTweener, float distanceUnit)
            {
                this.distanceUnit = distanceUnit;
                _timeTweener = timeTweener;
                _uiList = new Queue<UINoteProxy>();
            }

            public void RefreshParams(float distanceUnit)
            {
                this.distanceUnit = distanceUnit;
            }

            public void AddLast(Graphic graphic, float targetTime)
            {
                var note = new UINoteProxy(graphic, targetTime);
                _uiList.Enqueue(note);
                note.FadeIn();
            }

            public void RemoveFirst()
            {
                var dequeue = _uiList.Dequeue();
                dequeue.FadeOut();
            }

            public void HitFirst()
            {
                var dequeue = _uiList.Dequeue();
                dequeue.FadeHitStop();
            }

            public void Tick()
            {
                foreach (var uiNoteProxy in _uiList)
                {
                    uiNoteProxy.UpdateView(_timeTweener.TweenTime, distanceUnit);
                }
            }

            // proxy for monobehaviour
            private class UINoteProxy : SingleNote
            {
                public readonly Graphic graphic;
                public bool Stopped { get; private set; }

                private readonly RectTransform transform;

                public UINoteProxy(Graphic graphic, float targetTime) : base(targetTime)
                {
                    this.graphic = graphic;
                    transform = (RectTransform)graphic.transform;
                    // isHit = false;
                }

                public void FadeIn()
                {
                    graphic.DOFade(1, 0.4f);
                }

                public void FadeOut()
                {
                    graphic.DOFade(0, 0.1f);
                }

                public void FadeHitStop()
                {
                    Stopped = true;
                    graphic.DOFade(0, 0.2f);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void UpdateView(float time, float distanceUnit)
                {
                    if (Stopped) return;

                    var delta = realTime - time;
                    transform.localPosition = transform.localPosition.With(distanceUnit * delta);
                }
            }
        }
    }
}
