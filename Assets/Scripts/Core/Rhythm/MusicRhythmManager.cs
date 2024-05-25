using Shuile.Audio;
using Shuile.Utils;

using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Shuile.Framework;
using Shuile.Persistent;
using Shuile.ResourcesManagement.Loader;
using Shuile.Gameplay.Event;
using Shuile.Gameplay;

namespace Shuile.Rhythm.Runtime
{
    public interface IMusicRhythmManager
    {
        AudioPlayerInUnity AudioPlayer { get; }
        PreciseMusicPlayer PreciseMusicPlayer { get; }
        bool IsPlaying { get; }
        float CurrentTime { get; }
        float MusicLength { get; }
        bool IsMusicEnd { get; }
        void RefreshData();
        void StartPlay();
        void StopPlay();
        void RestartPlay();
        void FadeOutAndStop(float duration = 0.8f);
        void SetCurrentTime(float time);
    }

    //control the music play and music time progress, manage the rhythm check
    public class MusicRhythmManager : IMusicRhythmManager
    {
        private LevelConfigSO levelConfig;

        private ChartData currentChart;
        private bool isPlaying = false;

        private PreciseMusicPlayer preciseMusicPlayer;

        [HideInInspector] public bool playOnAwake = true;
        [HideInInspector] public float playTimeScale = 1f;
        [HideInInspector] public float volume = 0.4f;

        public AudioPlayerInUnity AudioPlayer => preciseMusicPlayer.AudioPlayer;
        public PreciseMusicPlayer PreciseMusicPlayer => preciseMusicPlayer;
        public bool IsPlaying => isPlaying;
        public float CurrentTime => preciseMusicPlayer.CurrentTime;
        public float MusicLength => currentChart.musicLength;
        public bool IsMusicEnd => CurrentTime >= MusicLength;

        public MusicRhythmManager()
        {
            preciseMusicPlayer = GameplayService.Interface.Get<PreciseMusicPlayer>();
            levelConfig = LevelResourcesLoader.Instance.SyncContext.levelConfig;

            LevelStartEvent.Register(name =>
            {
                preciseMusicPlayer = GameplayService.Interface.Get<PreciseMusicPlayer>();
                RefreshData();
                if (playOnAwake)
                    StartPlay();
            });
        }

        public void RefreshData()
        {
            currentChart = LevelDataBinder.Instance.ChartData;

            playOnAwake = levelConfig.playOnAwake;
            playTimeScale = levelConfig.playTimeScale;
            volume = levelConfig.volume;

            preciseMusicPlayer.Restore();
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
            preciseMusicPlayer.Restore();
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
