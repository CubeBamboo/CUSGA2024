using CbUtils;
using UnityEngine;
using Shuile.Audio;
using Shuile.Framework;
using Cysharp.Threading.Tasks;

namespace Shuile.Rhythm.Runtime
{
    //control the music play and music time progress, manage the rhythm check
    public class MusicRhythmManager : MonoSingletons<MusicRhythmManager>
    {
        private LevelConfigSO levelConfig;
        private ChartData currentChart;
        private PlayerSettingsConfigSO playerConfig;
        private float currentTime; // timer for music playing
        private bool isPlaying = false;

        private IAudioPlayer audioPlayer; // for music playing

        [HideInInspector] public bool playOnAwake = true;
        [HideInInspector] public float playTimeScale = 1f;
        [HideInInspector] public float volume = 0.4f;

        public bool IsPlaying => isPlaying;
        public float MissToleranceInSeconds => levelConfig.missTolerance * 0.001f;
        public float CurrentTime => currentTime;
        /// <summary>
        /// unit: second
        /// </summary>
        public float BpmInterval => 60f / currentChart.time[0].bpm;
        public float MusicBpm => currentChart.time[0].bpm;
        public float MusicOffsetInSeconds => currentChart.time[0].offset * 0.001f;

        protected override void OnAwake()
        {
            audioPlayer = MainGame.Interface.Get<IAudioPlayer>();
            currentChart = LevelResources.Instance.currentChart;
            levelConfig = LevelResources.Instance.levelConfig;
            playerConfig = LevelResources.Instance.playerConfig;
            playOnAwake = LevelResources.Instance.musicManagerConfig.playOnAwake;
            playTimeScale = LevelResources.Instance.musicManagerConfig.playTimeScale;
            volume = LevelResources.Instance.musicManagerConfig.volume;
        }

        private void Start()
        {
            InitMusic();
            currentTime = 0;
            if(playOnAwake)
                StartPlay();
        }

        private void FixedUpdate()
        {
            if (!isPlaying)
                return;

            currentTime += Time.fixedDeltaTime;
        }

        private void InitMusic()
        {
            audioPlayer.LoadClip(currentChart.audioClip);
            audioPlayer.Volume = volume;
        }

        public async void StartPlay()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f)); // delay to waiting for unity editor' play init
            float offsetInSeconds = (currentChart.time[0].offset + playerConfig.globalOffset) * 0.001f;
            Time.timeScale = playTimeScale;
            audioPlayer.Pitch = playTimeScale;

            // play clip
            audioPlayer.Play();

            currentTime = -offsetInSeconds;
            isPlaying = true; // -> start timing
        }
    }
}
