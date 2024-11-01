using UnityEngine;

namespace Shuile
{
    public class BackgroundMusicChannel : MonoBehaviour
    {
        private PlainAudioChannel _audioChannel;

        private void Awake()
        {
            _audioChannel = new PlainAudioChannel(gameObject);
            _audioChannel.Source.loop = true;
        }

        public AudioSource Source => _audioChannel.Source;
        public void Play(AudioClip clip) => _audioChannel.Play(clip);
    }
}
