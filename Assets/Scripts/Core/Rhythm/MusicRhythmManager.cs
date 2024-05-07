using CbUtils;
using Shuile.Audio;
using Shuile.Utils;

using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Shuile.Framework;
using Shuile.Persistent;
using Shuile.Gameplay;

namespace Shuile.Rhythm.Runtime
{
    //control the music play and music time progress, manage the rhythm check
    public class MusicRhythmManager : MonoNonAutoSpawnSingletons<MusicRhythmManager>
    {
        private ChartData currentChart;
        private PlayerSettingsConfigSO playerConfig;
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

        private void Start()
        {
            preciseMusicPlayer = new(new SimpleAudioPlayer());
            RefreshData();
            if (playOnAwake)
                StartPlay();
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
            
            var resources = LevelResources.Instance;
            playerConfig = resources.playerConfig;
            playOnAwake = resources.musicManagerConfig.playOnAwake;
            playTimeScale = resources.musicManagerConfig.playTimeScale;
            volume = resources.musicManagerConfig.volume;

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
