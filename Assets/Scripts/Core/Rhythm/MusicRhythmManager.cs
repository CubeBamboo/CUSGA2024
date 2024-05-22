using CbUtils;
using Shuile.Audio;
using Shuile.Utils;

using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Shuile.Framework;
using Shuile.Persistent;
using Shuile.Gameplay;
using Shuile.ResourcesManagement.Loader;
using System.Threading.Tasks;
using CbUtils.Kits.Tasks;
using Shuile.Gameplay.Event;

namespace Shuile.Rhythm.Runtime
{
    //control the music play and music time progress, manage the rhythm check
    public class MusicRhythmManager : MonoSingletons<MusicRhythmManager>
    {
        private LevelConfigSO levelConfig;

        private ChartData currentChart;
        private bool isPlaying = false;

        public LevelModel levelModel => GameplayService.Interface.LevelModel;

        private PreciseMusicPlayer preciseMusicPlayer;

        [HideInInspector] public bool playOnAwake = true;
        [HideInInspector] public float playTimeScale = 1f;
        [HideInInspector] public float volume = 0.4f;

        public AudioPlayerInUnity AudioPlayer => preciseMusicPlayer.AudioPlayer;
        public bool IsPlaying => isPlaying;
        public float CurrentTime => preciseMusicPlayer.CurrentTime;
        public float MusicLength => currentChart.musicLength;
        public bool IsMusicEnd => CurrentTime >= MusicLength;

        protected override void OnAwake()
        {
            preciseMusicPlayer = new(new SimpleAudioPlayer());
            levelConfig = LevelResourcesLoader.Instance.SyncContext.levelConfig;
            RefreshData();
        }
        private void Start()
        {
            LevelStartEvent_AutoClear.Register(name =>
            {
                if (playOnAwake)
                    StartPlay();
            });
        }

        private void FixedUpdate()
        {
            preciseMusicPlayer.CallTickAction();
        }

        private void OnDestroy()
        {
            preciseMusicPlayer.Reset();
        }

        public void RefreshData()
        {
            currentChart = LevelDataBinder.Instance.ChartData;

            playOnAwake = levelConfig.playOnAwake;
            playTimeScale = levelConfig.playTimeScale;
            volume = levelConfig.volume;

            preciseMusicPlayer.Reset();
            preciseMusicPlayer.LoadClip(currentChart.audioClip);
        }

        public void StartPlay()
        {
            float offsetInSeconds = (currentChart.time[0].offset + MainGame.Interface.Get<Config>().GlobalDelay) * 0.001f;
            Time.timeScale = playTimeScale;
            preciseMusicPlayer.Volume = volume;

            preciseMusicPlayer.Play(offsetInSeconds).Forget();
        }

        public void StopPlay()
        {
            preciseMusicPlayer.Stop();
        }

        public void RestartPlay()
        {
            preciseMusicPlayer.Reset();
            preciseMusicPlayer.LoadClip(currentChart.audioClip);
            StartPlay();
        }

        public void FadeOutAndStop(float duration = 0.8f)
        {
            var targetAudioPlayer = AudioPlayer;
            targetAudioPlayer.TargetSource.DOFade(0, duration).OnComplete(() =>
            {
                targetAudioPlayer.Stop();
            });
        }

        public void SetCurrentTime(float time)
            => preciseMusicPlayer.SetCurrentTime(time);
    }
}
