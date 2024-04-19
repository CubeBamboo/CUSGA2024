using CbUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile.Audio
{
    public class SimpleAudioPlayer : IAudioPlayer
    {
        private bool isPaused = false;

        public float Time
        {
            get => AudioManager.Instance.OtherSource.time;
            set => AudioManager.Instance.OtherSource.time = value;
        }
        public float Volume
        {
            get => AudioManager.Instance.OtherSource.volume;
            set => AudioManager.Instance.OtherSource.volume = value;
        }
        public float Pitch
        {
            get => AudioManager.Instance.OtherSource.pitch;
            set => AudioManager.Instance.OtherSource.pitch = value;
        }

        // custom
        public AudioSource Source => AudioManager.Instance.OtherSource;

        public void LoadClip(AudioClip clip)
        {
            AudioManager.Instance.OtherSource.clip = clip;
        }

        public void Pause()
        {
            if (isPaused)
                AudioManager.Instance.OtherSource.UnPause();
            else
                AudioManager.Instance.OtherSource.Pause();

            isPaused = !isPaused;
        }

        public void Play()
        {
            AudioManager.Instance.OtherSource.Play();
        }

        public void PlayScheduled(double time)
        {
            AudioManager.Instance.OtherSource.PlayScheduled(time);
        }

        public void Reset()
        {
            var targetSource = AudioManager.Instance.OtherSource;
            AudioManager.Instance.OtherSource.Stop();
            targetSource.clip = null;
            targetSource.volume = 1f;
            isPaused = false;
        }

        public void Stop()
        {
            AudioManager.Instance.OtherSource.Stop();
        }
    }
}
