using CbUtils.Extension;
using Shuile.Core.Gameplay;
using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.MonoGadget;
using Shuile.Persistent;
using Shuile.ResourcesManagement.Loader;
using Shuile.Rhythm.Runtime;

using System.Linq;
using System.Runtime.CompilerServices;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Shuile.UI
{
    public class LatencyTestPlane : BasePanelWithMono
    {
        private struct Note
        {
            public RectTransform transform;
            public Graphic graphic;
            public float judgeTime;
            public float? clickTime;

            public Note(RectTransform transform, float judgeTime)
            {
                this.transform = transform;
                this.graphic = transform.GetComponent<Graphic>();
                this.judgeTime = judgeTime;
                this.clickTime = null;
            }

            public Note(float judgeTime)
            {
                this.transform = null;
                this.graphic = null;
                this.judgeTime = judgeTime;
                this.clickTime = null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ApplyPositionX(float x) => transform.localPosition = transform.localPosition.With(x: x);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void DestroySafe()
            {
                if (transform != null)
                    Destroy(transform.gameObject);
            }
        }

        // UI Display
        [SerializeField] private RectTransform latencyIndicator;
        [SerializeField] private TMP_Text latencyText;
        [SerializeField] private float distanceUnit = 100;
        [SerializeField] private RectTransform noteParent;
        private Note[] notes;

        // Play component
        private MusicRhythmManager musicRhythmManager;
        private MusicTimeTweener timeTweener;
        private bool isPlaying = false;
        private int detectingNoteIndex = 0;

        // Result Display
        private Note testResultIndicator;
        [SerializeField] private TMP_Text testResultText;
        private int testResultLatency = 0;

        private Viewer<Config> configViewer;
        [SerializeField] private string levelLabel;
        public static readonly int maxLatency = 800;

        public string LevelLabel
        {
            get => levelLabel;
            set => levelLabel = value;
        }

        private void Awake()
        {
            this.RegisterUI<LatencyTestPlane>();
        }
        private void OnDestroy()
            => this.UnRegisterUI<LatencyTestPlane>();

        private void Update()
        {
            if (!isPlaying) return;

            if (detectingNoteIndex < notes.Length)
            {
                var currTime = timeTweener.TweenTime;
                var delta = currTime - notes[detectingNoteIndex].judgeTime;
                notes[detectingNoteIndex].ApplyPositionX(delta * distanceUnit);

                if (delta > maxLatency / 1000f)
                {
                    // Miss
                    notes[detectingNoteIndex].graphic.CrossFadeAlpha(0f, 0.1f, false);
                    notes[detectingNoteIndex++].clickTime = null;
                    return;
                }

                if (Input.GetMouseButtonDown(0) && Mathf.Abs(delta) <= maxLatency / 1000f)
                {
                    notes[detectingNoteIndex].graphic.CrossFadeAlpha(0.5f, 0.2f, false);
                    notes[detectingNoteIndex++].clickTime = currTime;
                    return;
                }
            }
            else
            {
                isPlaying = false;
                musicRhythmManager.StopPlay();
                // 结算
                if (!TryCalcTestDelay(out var delay))
                {
                    testResultText.gameObject.SetActive(false);
                    return;
                }
                testResultText.gameObject.SetActive(true);

                testResultLatency = (int)(delay * 1000);
                testResultIndicator.ApplyPositionX(delay * distanceUnit);
                testResultIndicator.graphic.CrossFadeAlpha(1f, 0.2f, false);
                testResultText.rectTransform.localPosition = testResultText.rectTransform.localPosition.With(x: delay * distanceUnit);
                testResultText.text = "Result: " + testResultLatency.ToString() + "ms";
            }
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
            musicRhythmManager.StopPlay();
            isPlaying = false;
            foreach (var note in notes)
                note.DestroySafe();
            testResultIndicator.DestroySafe();
            timeTweener.DestroySafe();
            musicRhythmManager.DestroySafe();
            GameplayService.Interface.OnDeInit();
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            // Create a playing scope to play music and chart
            var level = GameResourcesLoader.Instance.GetLevelDataMapAsync().Result.GetLevelData(LevelLabel);
            LevelDataBinder.Instance.SetLevelData(level);
            GameplayService.Interface.OnInit();

            musicRhythmManager = this.gameObject.GetOrAddComponent<MusicRhythmManager>();
            musicRhythmManager.RefreshData();
            musicRhythmManager.playOnAwake = false;
            musicRhythmManager.StopPlay();
            timeTweener = musicRhythmManager.gameObject.GetOrAddComponent<MusicTimeTweener>();

            if (notes == null)
            {
                var chart = ChartDataCreator.CreateLatencyTestDefault();
                notes = chart.note.Select(note => note.ToPlayTime()).Select(time => new Note(time)).ToArray();
            }

            latencyIndicator.localPosition = latencyIndicator.localPosition.With(x: configViewer.Data.GlobalDelay / 1000f * distanceUnit);
            latencyText.text = configViewer.Data.GlobalDelay.ToString() + "ms";
            ResetTest();
        }

        public override void Init()
        {
            base.Init();

            configViewer = MainGame.Interface.CreatePersistentDataViewer<Config, MainGame>();
            configViewer.AutoSave = true;
        }

        public override void DeInit()
        {
            base.DeInit();
            notes = null;
            configViewer = null;
        }

        private bool TryCalcTestDelay(out float delay)
        {
            (int clickedCount, float delayTotal) = notes
                .Where(n => n.clickTime != null)
                .Aggregate<Note, (int click, float delay)>(
                    (0, 0f),
                    (old, note) => (old.click + 1, old.delay + note.clickTime.Value - note.judgeTime)
                );
            delay = clickedCount == 0 ? 0f : delayTotal / clickedCount;
            return clickedCount != 0;
        }

        #region Unity Button Actions
        public void StartTest()
        {
            if (isPlaying)
                return;

            isPlaying = true;
            int savedDelay = configViewer.Data.GlobalDelay;
            configViewer.Data.GlobalDelay = 0;
            musicRhythmManager.SetCurrentTime(0);
            musicRhythmManager.StartPlay();
            configViewer.Data.GlobalDelay = savedDelay;
        }

        public void ResetTest()
        {
            if (isPlaying)
                return;

            var prefab = LevelResourcesLoader.Instance.SyncContext.globalPrefabs.noteIndicator;
            for (int i = 0; i < notes.Length; i++)
            {
                if (notes[i].transform == null)
                    notes[i] = new((RectTransform)Instantiate(prefab, noteParent).transform, notes[i].judgeTime);
                notes[i].graphic.CrossFadeAlpha(1f, 0f, true);
                notes[i].ApplyPositionX(114514f);
                notes[i].clickTime = null;
            }
            detectingNoteIndex = 0;
            musicRhythmManager.StopPlay();

            testResultIndicator.DestroySafe();
            testResultIndicator = new((RectTransform)Instantiate(prefab, noteParent).transform, 0f);
            testResultIndicator.graphic.color = testResultText.color.WithAlpha(1f);
            testResultIndicator.graphic.CrossFadeAlpha(0f, 0f, true);
            testResultText.text = string.Empty;
            testResultText.gameObject.SetActive(false);
        }

        public void AddLatency(int value)
        {
            if (isPlaying)
                return;
            configViewer.Data.GlobalDelay += value;
            latencyIndicator.localPosition = latencyIndicator.localPosition.With(x: configViewer.Data.GlobalDelay / 1000f * distanceUnit);
            latencyText.text = configViewer.Data.GlobalDelay.ToString() + "ms";
        }

        public void ApplyTestLatency()
        {
            AddLatency(testResultLatency - configViewer.Data.GlobalDelay);
        }
        #endregion
    }
}
