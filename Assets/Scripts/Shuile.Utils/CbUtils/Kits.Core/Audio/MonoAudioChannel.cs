using UnityEngine;
using UnityEngine.Audio;

namespace Shuile
{
    public class MonoAudioChannel : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup audioMixerGroup;

        private PlainAudioChannel _audioChannel;

        private void Awake()
        {
            _audioChannel = new PlainAudioChannel(gameObject, audioMixerGroup);
        }

        public void Play(string addressableKey) => _audioChannel.Play(addressableKey);

        public void Play(AudioClip clip) => _audioChannel.Play(clip);
    }

    public class PlainAudioChannel
    {
        private ResourceLoader _resourceLoader;

        public AudioSource Source { get; }

        public PlainAudioChannel(GameObject gameObject, AudioMixerGroup audioMixerGroup = null)
        {
            Source = gameObject.AddComponent<AudioSource>();
            if(audioMixerGroup != null) Source.outputAudioMixerGroup = audioMixerGroup;

            _resourceLoader = new ResourceLoader();
        }

        public void Play(string addressableKey)
        {
            var clip = _resourceLoader.Load<AudioClip>(addressableKey);
            Play(clip);
        }

        public void Play(AudioClip clip)
        {
            Source.clip = clip;
            Source.Play();
        }
    }
}
