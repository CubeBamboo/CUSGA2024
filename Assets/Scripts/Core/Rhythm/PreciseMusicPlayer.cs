using Cysharp.Threading.Tasks;
using Shuile.Audio;
using Shuile.Gameplay;
using System.Threading;
using UnityEngine;

namespace Shuile.Utils
{
    /// <summary>
    /// it provide a async player for lower delay, a timer start from music's specific position
    /// </summary>
    public class PreciseMusicPlayer : MonoBehaviour
    {
        private AudioPlayerInUnity audioPlayer;
        public AudioPlayerInUnity AudioPlayer => audioPlayer;

        private void Awake()
        {
            GameplayService.Interface.Register<PreciseMusicPlayer>(this);
            audioPlayer = new SimpleAudioPlayer();
            Restore();
        }
        private void OnDestroy()
        {
            GameplayService.Interface.UnRegister<PreciseMusicPlayer>();
        }

        public void FixedUpdate()
        {
            if (!IsPlaying) return;
            CurrentTime += Time.fixedDeltaTime;
        }

        /// <summary> timer for music playing by play offset </summary>
        public float CurrentTime { get; private set; } = 0f;
        public bool IsPlaying { get; set; } = false;
        public float PlayTimeScale { get; set; } = 1f;
        public float Volume
        {
            get => audioPlayer.Volume;
            set => audioPlayer.Volume = value;
        }

        private CancellationTokenSource asyncPlayTokenSource;

        public void LoadClip(AudioClip clip)
        {
            audioPlayer.LoadClip(clip);
        }

        public async UniTask Play(float offsetInSeconds, float startDelay = 0.5f)
        {
            asyncPlayTokenSource = new();
            
            await UniTask.Delay(System.TimeSpan.FromSeconds(startDelay), cancellationToken: asyncPlayTokenSource.Token);
            audioPlayer.Pitch = PlayTimeScale;

            var playAt = AudioSettings.dspTime + 1f;
            var audioDelta = await audioPlayer.WaitPlayScheduled(playAt, asyncPlayTokenSource.Token);

            CurrentTime = -offsetInSeconds + (float)audioDelta;
            IsPlaying = true; // -> start timing
        }
        public void PlayImmediatly(float offset)
        {
            audioPlayer.Pitch = PlayTimeScale;
            audioPlayer.Play();
            CurrentTime = -offset;
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
            audioPlayer.Stop();
            asyncPlayTokenSource?.Cancel();
        }

        /// <summary>
        /// default need to call in FixedUpdate
        /// </summary>

        public void Restore()
        {
            CurrentTime = 0;
            IsPlaying = false;
            audioPlayer.Reset();
        }

        public void SetCurrentTime(float time)
        {
            time = Mathf.Clamp(time, 0f, audioPlayer.TargetSource.clip.length);

            CurrentTime = time;
            audioPlayer.TargetSource.time = time;
        }
    }
}
