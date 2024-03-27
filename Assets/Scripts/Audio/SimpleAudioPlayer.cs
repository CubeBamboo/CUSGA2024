using CbUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile.Audio
{
    public class SimpleAudioPlayer : IAudioPlayer
    {
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

        public void LoadClip(AudioClip clip)
        {
            AudioManager.Instance.OtherSource.clip = clip;
        }

        public void Pause()
        {
            AudioManager.Instance.OtherSource.Pause();
        }

        public void Play()
        {
            AudioManager.Instance.OtherSource.Play();
        }

        public void PlayScheduled(double time)
        {
            AudioManager.Instance.OtherSource.PlayScheduled(time);
        }
    }
}
