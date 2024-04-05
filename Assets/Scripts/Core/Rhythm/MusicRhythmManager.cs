using CbUtils;
using UnityEngine;
using Shuile.Audio;
using Shuile.Framework;
using Cysharp.Threading.Tasks;

namespace Shuile.Rhythm
{
    //control the music play and music time progress, manage the rhythm check
    public class MusicRhythmManager : MonoSingletons<MusicRhythmManager>
    {
        [SerializeField] private LevelConfigSO levelConfig;
        [SerializeField] private MusicConfigSO currentMusic;
        [SerializeField] private PlayerSettingsConfigSO playerConfig;
        private float currentTime; // timer for music playing
        private bool isPlaying = false;

        private IAudioPlayer audioPlayer; // for music playing

        public bool playOnAwake = true;
        public float playTimeScale = 1f;
        public float volume = 0.4f;

        public bool IsPlaying => isPlaying;
        public float MissToleranceInSeconds => levelConfig.missTolerance * 0.001f;
        public float CurrentTime => currentTime;
        /// <summary>
        /// unit: second
        /// </summary>
        public float BpmInterval => 60f / currentMusic.bpm;
        public float MusicBpm => currentMusic.bpm;
        public float MusicOffsetInSeconds => currentMusic.offset * 0.001f;

        protected override void Awake()
        {
            base.Awake();

            MainGame.Interface.TryGet(out audioPlayer);
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
            audioPlayer.LoadClip(currentMusic.clip);
            audioPlayer.Volume = volume;
        }

        public async void StartPlay()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f)); // delay to waiting for unity editor' play init
            float offsetInSeconds = (currentMusic.offset + playerConfig.globalOffset) * 0.001f;
            Time.timeScale = playTimeScale;
            audioPlayer.Pitch = playTimeScale;

            // play clip
            audioPlayer.Play();

            currentTime = -offsetInSeconds;
            isPlaying = true; // -> start timing
        }
    }
}
