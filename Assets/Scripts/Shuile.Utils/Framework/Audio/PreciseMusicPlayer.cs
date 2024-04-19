using CbUtils;
using CbUtils.LinkedValue;
using Cysharp.Threading.Tasks;
using Shuile.Audio;
using UnityEngine;

namespace Shuile.Utils
{
    /// <summary>
    /// it provide a async player for lower delay, a timer start from music's specific position
    /// </summary>
    public class PreciseMusicPlayer
    {
        private readonly IAudioPlayer audioPlayer;
        private System.Action tickAction;

        public IAudioPlayer AudioPlayer => audioPlayer;

        public PreciseMusicPlayer(IAudioPlayer audioPlayer)
        {
            this.audioPlayer = audioPlayer;

            tickAction = () =>
            {
                if (!IsPlaying)
                    return;

                CurrentTime += Time.fixedDeltaTime;
            };

            Reset();
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

        public void LoadClip(AudioClip clip)
        {
            audioPlayer.LoadClip(clip);
        }

        public async UniTask Play(float offsetInSeconds, float startDelay = 0.5f)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(startDelay));
            audioPlayer.Pitch = PlayTimeScale;

            var playAt = AudioSettings.dspTime + 1f;
            var audioDelta = await audioPlayer.WaitPlayScheduled(playAt);

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
        }

        public void CustomTicker(System.Action action)
        {
            tickAction = action;
        }

        /// <summary>
        /// default need to call in FixedUpdate
        /// </summary>
        public void CallTickAction()
        {
            tickAction.Invoke();
        }

        public void Reset()
        {
            CurrentTime = 0;
            IsPlaying = false;
            audioPlayer.Reset();
        }
    }
}
