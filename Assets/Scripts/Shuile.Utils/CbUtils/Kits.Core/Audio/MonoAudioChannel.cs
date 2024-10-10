using UnityEngine;
using UnityEngine.Audio;

namespace Shuile
{
    public class MonoAudioChannel : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup audioMixerGroup;

        private AudioSource _audioSource;
        private ResourceLoader _resourceLoader;

        private void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.outputAudioMixerGroup = audioMixerGroup;

            _resourceLoader = new ResourceLoader();
        }

        public void Play(string addressableKey)
        {
            var clip = _resourceLoader.Load<AudioClip>(addressableKey);
            Play(clip);
        }

        public void Play(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }
}
