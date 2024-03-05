using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CbUtils
{
    //maybe it work well with UnityEvent
    public class MonoAudioTrigger : MonoBehaviour
    {
        public enum Channel { BGM, SFX, VOICE }

        public AudioClip clip;
        public bool PlayOnAwake;
        public Channel channel;

        private void Start()
        {
            if(PlayOnAwake) Play();
        }

        public void Trigger()
        {
            Play();
        }

        private void Play()
        {
            switch (channel)
            {
                case Channel.BGM:
                    AudioManager.Instance.PlayBgm(clip);
                    break;
                case Channel.SFX:
                    AudioManager.Instance.PlaySFX(clip);
                    break;
                case Channel.VOICE:
                    AudioManager.Instance.PlayVoice(clip);
                    break;
            }
        }
    }
}