using CbUtils;
using UnityEngine;
using Shuile.Audio;
using Shuile.Utils;

namespace Shuile.Rhythm.Runtime
{
    //control the music play and music time progress, manage the rhythm check
    public class MusicRhythmManager : MonoSingletons<MusicRhythmManager>
    {
        private LevelConfigSO levelConfig;
        private ChartData currentChart;
        private PlayerSettingsConfigSO playerConfig;
        private bool isPlaying = false;

        private PreciseMusicPlayer preciseMusicPlayer;

        [HideInInspector] public bool playOnAwake = true;
        [HideInInspector] public float playTimeScale = 1f;
        [HideInInspector] public float volume = 0.4f;

        public IAudioPlayer AudioPlayer => preciseMusicPlayer.AudioPlayer;
        public bool IsPlaying => isPlaying;
        public float MissToleranceInSeconds => levelConfig.missTolerance * 0.001f;
        public float CurrentTime => preciseMusicPlayer.CurrentTime;
        /// <summary>
        /// unit: second
        /// </summary>
        public float BpmInterval => 60f / currentChart.time[0].bpm;
        public float MusicBpm => currentChart.time[0].bpm;
        public float MusicOffsetInSeconds => currentChart.time[0].offset * 0.001f;

        protected override void OnAwake()
        {
            preciseMusicPlayer = new(new SimpleAudioPlayer());
            currentChart = LevelDataBinder.Instance.chartData;

            var resources = LevelResources.Instance;
            levelConfig = resources.levelConfig;
            playerConfig = resources.playerConfig;
            playOnAwake = resources.musicManagerConfig.playOnAwake;
            playTimeScale = resources.musicManagerConfig.playTimeScale;
            volume = resources.musicManagerConfig.volume;
        }

        private void Start()
        {
            preciseMusicPlayer.Reset();
            preciseMusicPlayer.LoadClip(currentChart.audioClip);

            if(playOnAwake)
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

        public void StartPlay()
        {
            float offsetInSeconds = (currentChart.time[0].offset + playerConfig.globalOffset) * 0.001f;
            Time.timeScale = playTimeScale;
            preciseMusicPlayer.Volume = volume;

#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            preciseMusicPlayer.Play(offsetInSeconds);
#pragma warning restore CS4014
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
    }
}
