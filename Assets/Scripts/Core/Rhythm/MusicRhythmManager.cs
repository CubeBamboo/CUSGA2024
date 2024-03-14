using CbUtils;
using UnityEngine;
using Shuile.Audio;
using Shuile.Framework;

namespace Shuile.Rhythm
{
    //control the music play and music time progress
    public class MusicRhythmManager : MonoSingletons<MusicRhythmManager>
    {
        [SerializeField] private LevelConfigSO levelConfig;
        [SerializeField] private MusicConfigSO currentMusic;
        [SerializeField] private PlayerSettingsConfigSO playerConfig;
        private float currentTime; // timer for music playing
        private bool isPlaying = false;

        private IAudioPlayer audioPlayer; // for music playing

        public bool playOnAwake = true;

        public bool IsPlaying => isPlaying;
        public float MissTolerance => levelConfig.missTolerance;
        public float CurrentTime => currentTime;
        public float BpmInterval => 60f / currentMusic.bpm;
        public float MusicBpm => currentMusic.bpm;
        public float MusicOffset => currentMusic.offset;

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
            audioPlayer.Volume = 0.4f;
        }

        public void StartPlay()
        {
            float offsetInSeconds = (currentMusic.offset + playerConfig.globalOffset) * 0.001f;

            // play clip
            audioPlayer.Play();

            currentTime = -offsetInSeconds;
            isPlaying = true; // -> start timing

        }
    }
}
