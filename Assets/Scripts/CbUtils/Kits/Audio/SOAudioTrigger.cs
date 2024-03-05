using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CbUtils
{
    //maybe it work well with UnityEvent
    [CreateAssetMenu(menuName = "Custom/AudioTrigger")]
    public class SOAudioTrigger : ScriptableObject
    {
        public enum Channel { BGM, SFX, VOICE }

        public AudioClip clip;
        public Channel channel;

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